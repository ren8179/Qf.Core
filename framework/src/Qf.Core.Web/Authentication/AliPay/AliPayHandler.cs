using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Aop.Api;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Domain;

namespace Qf.Core.Web.Authentication.AliPay
{
    internal class AliPayHandler : OAuthHandler<AliPayOptions>
    {
        private readonly ISecureDataFormat<AuthenticationProperties> _secureDataFormat;
        private readonly ILogger _logger;
        /// <summary>
        /// Called after options/events have been initialized for the handler to finish initializing itself.
        /// </summary>
        /// <returns>A task</returns>
        protected override async Task InitializeHandlerAsync()
        {
            await base.InitializeHandlerAsync();
            if (Options.UseCachedStateDataFormat)
            {
                Options.StateDataFormat = _secureDataFormat;
            }
        }

        public AliPayHandler(
             IOptionsMonitor<AliPayOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ISecureDataFormat<AuthenticationProperties> secureDataFormat)
            : base(options, logger, encoder, clock)
        {
            _secureDataFormat = secureDataFormat;
            _logger = logger.CreateLogger(nameof(AliPayHandler));
        }
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (string.IsNullOrEmpty(properties.RedirectUri))
            {
                properties.RedirectUri = OriginalPathBase + OriginalPath + Request.QueryString;
            }

            // OAuth2 10.12 CSRF
            GenerateCorrelationId(properties);

            var authorizationEndpoint = BuildChallengeUrl(properties, BuildRedirectUri(Options.CallbackPath));
            var redirectContext = new RedirectContext<OAuthOptions>(
                Context, Scheme, Options,
                properties, authorizationEndpoint);
            _logger.LogDebug($"RedirectUri=> {redirectContext.RedirectUri}");
            await Events.RedirectToAuthorizationEndpoint(redirectContext);

            var location = Context.Response.Headers[HeaderNames.Location];
            if (location == StringValues.Empty)
            {
                location = "(not set)";
            }
            var cookie = Context.Response.Headers[HeaderNames.SetCookie];
            if (cookie == StringValues.Empty)
            {
                cookie = "(not set)";
            }
            _logger.LogDebug($"AliPayHandler HandleChallenge with Location: {location}; and Set-Cookie: {cookie}.");
        }

        /*
         * Challenge 盘问握手认证协议
         * 这个词有点偏，好多翻译工具都查不出。
         * 这个解释才是有些靠谱 http://abbr.dict.cn/Challenge/CHAP
         */
        /// <summary>
        /// 构建请求CODE的Url地址（这是第一步，准备工作）
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            var scope = FormatScope();

            var state = Options.StateDataFormat.Protect(properties);

            if (!string.IsNullOrEmpty(Options.CallbackUrl))
            {
                redirectUri = Options.CallbackUrl;
            }

            var parameters = new Dictionary<string, string>()
            {
                { "app_id",  properties.Items["AliPayAppid"] },
                { "redirect_uri", redirectUri },
                //{ "scope", scope },
                //{ "state", state },
            };
            var ret = QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, parameters);
            //scope 不能被UrlEncode
            ret += $"&scope={scope}&state={state}";
            _logger.LogDebug("请求CODE " + ret);
            return ret;
        }
        /// <summary>
        /// 处理微信授权结果（接收微信授权的回调）
        /// </summary>
        /// <returns></returns>
        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            //第一步，处理工作
            var query = Request.Query;
            var code = query["auth_code"];
            var state = query["state"];
            _logger.LogDebug($"接收支付宝授权的回调 auth_code:{code} state:{state}");

            if (StringValues.IsNullOrEmpty(code))
            {
                return HandleRequestResult.Fail("Auth_code was not found.");
            }

            if (StringValues.IsNullOrEmpty(state))
            {
                return HandleRequestResult.Fail("State was not found.");
            }

            var properties = Options.StateDataFormat.Unprotect(state);
            if (properties == null)
            {
                return HandleRequestResult.Fail("认证状态(state)丢失或无效.");
            }

            // OAuth2 10.12 CSRF
            if (!ValidateCorrelationId(properties))
            {
                _logger.LogWarning("Correlation failed.");
                //return HandleRequestResult.Fail("Correlation failed.");
            }

            //第二步，通过Code获取Access Token
            var redirectUrl = !string.IsNullOrEmpty(Options.CallbackUrl) ?
                Options.CallbackUrl :
                BuildRedirectUri(Options.CallbackPath);
            var codeExchangeContext = new OAuthCodeExchangeContext(properties, code, redirectUrl);
            using var tokens = await ExchangeCodeAsync(codeExchangeContext);

            if (tokens.Error != null)
            {
                return HandleRequestResult.Fail(tokens.Error, properties);
            }

            if (string.IsNullOrEmpty(tokens.AccessToken))
            {
                return HandleRequestResult.Fail("获取 access token 失败.", properties);
            }

            var identity = new ClaimsIdentity(ClaimsIssuer);

            if (Options.SaveTokens)
            {
                var authTokens = new List<AuthenticationToken>();

                authTokens.Add(new AuthenticationToken { Name = "access_token", Value = tokens.AccessToken });
                if (!string.IsNullOrEmpty(tokens.RefreshToken))
                {
                    authTokens.Add(new AuthenticationToken { Name = "refresh_token", Value = tokens.RefreshToken });
                }

                if (!string.IsNullOrEmpty(tokens.TokenType)) //微信就没有这个
                {
                    authTokens.Add(new AuthenticationToken { Name = "token_type", Value = tokens.TokenType });
                }

                if (!string.IsNullOrEmpty(tokens.ExpiresIn))
                {
                    int value;
                    if (int.TryParse(tokens.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                    {
                        // https://www.w3.org/TR/xmlschema-2/#dateTime
                        // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
                        var expiresAt = Clock.UtcNow + TimeSpan.FromSeconds(value);
                        authTokens.Add(new AuthenticationToken
                        {
                            Name = "expires_at",
                            Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                        });
                    }
                }

                properties.StoreTokens(authTokens);
            }

            var ticket = await CreateTicketAsync(identity, properties, tokens);
            if (ticket != null)
            {
                return HandleRequestResult.Success(ticket);
            }
            else
            {
                return HandleRequestResult.Fail("无法从远程服务器获取用户信息", properties);
            }
        }

        /// <summary>
        /// 通过Code获取Access Token(这是第二步) 
        /// </summary>
        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {
            _logger.LogDebug($"{Options.AppId} code换取access_token");
            IAopClient client = new DefaultAopClient(Options.TokenEndpoint, Options.AppId, Options.PrivateKey, "json", "1.0", "RSA2", Options.PublicKey, "utf-8", false);
            AlipaySystemOauthTokenRequest requestAccess_token = new AlipaySystemOauthTokenRequest();
            requestAccess_token.GrantType = "authorization_code";
            requestAccess_token.Code = context.Code;
            AlipaySystemOauthTokenResponse response = client.Execute(requestAccess_token);
            if (!response.IsError)
            {
                var result = $"{{ \"access_token\":\"{response.AccessToken}\", \"token_type\":\"bearer\", \"refresh_token\":\"{response.RefreshToken}\", \"expires_in\":\"{response.ExpiresIn}\", \"user_id\":\"{response.UserId}\"}}";
                _logger.LogDebug(response.Body);
                var payload = JsonDocument.Parse(result);
                if (payload.RootElement.TryGetProperty("errcode", out JsonElement errcode))
                {
                    return await Task.FromResult(OAuthTokenResponse.Failed(new Exception($"获取支付宝AccessToken出错。{errcode}")));
                }
                return OAuthTokenResponse.Success(payload);
            }
            else
            {
                _logger.LogDebug(response.Msg);
                return OAuthTokenResponse.Failed(new Exception($"获取支付宝AccessToken出错。原因：{response.Msg}"));
            }
        }

        /// <summary>
        /// 创建身份票据(这是第三步) 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="properties"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, tokens.Response.RootElement);
            context.RunClaimActions();
            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }

        /// <summary>
        /// 返回Scope
        /// </summary>
        /// <returns></returns>
        protected override string FormatScope()
        {
            return string.Join(",", Options.Scope);
        }
    }
}
