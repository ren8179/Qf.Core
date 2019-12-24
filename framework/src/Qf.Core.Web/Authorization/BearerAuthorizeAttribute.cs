using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qf.Core.Web.Authorization
{
    /// <summary>
    /// 授权认证
    /// </summary>
    public class BearerAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        /// <summary>
        /// 默认授权
        /// </summary>
        public const string DefaultAuthenticationScheme = "BearerAuth";
        private readonly ILogger<BearerAuthorizeAttribute> _logger;
        private readonly AppSettings _options;
        private readonly IHttpClientFactory _clientFactory;

        /// <summary>
        /// 认证服务器地址
        /// </summary>
        private readonly string issUrl = string.Empty;

        /// <summary>
        /// 保护的API名称
        /// </summary>
        private readonly string apiName = string.Empty;

        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="BearerAuthorizeAttribute"/> class.
        /// </summary>
        /// <param name="optionsAccessor"></param>
        /// <param name="clientFactory"></param>
        /// <param name="memoryCache"></param>
        public BearerAuthorizeAttribute(ILogger<BearerAuthorizeAttribute> logger, IOptions<AppSettings> optionsAccessor, IHttpClientFactory clientFactory, IMemoryCache memoryCache)
        {
            _logger = logger;
            AuthenticationSchemes = DefaultAuthenticationScheme;
            _options = optionsAccessor.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _cache = memoryCache;
            issUrl = _options.Auth.ServerUrl;
            apiName = _options.Auth.ApiName;
        }

        /// <summary>
        /// 授权认证
        /// </summary>
        /// <param name="context"></param>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
                return;

            var result = await context.HttpContext.AuthenticateAsync(DefaultAuthenticationScheme);
            if (result == null || !result.Succeeded)
            {
                string authHeader = context.HttpContext.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Bearer "))
                {
                    try
                    {
                        var token = authHeader.Replace("Bearer ", string.Empty);
                        if (!_cache.TryGetValue(token, out ClaimsPrincipal cacheEntry))
                        {
                            using (var httpclient = _clientFactory.CreateClient())
                            {
                                var jwtKey = await httpclient.GetStringAsync(issUrl + "/.well-known/openid-configuration/jwks");
                                var ids4keys = JsonSerializer.Deserialize<Ids4Keys>(jwtKey);
                                var handler = new JwtSecurityTokenHandler();
                                cacheEntry = handler.ValidateToken(token, new TokenValidationParameters
                                {
                                    ValidIssuer = issUrl,
                                    IssuerSigningKeys = ids4keys.Keys,
                                    ValidateLifetime = true,
                                    ValidAudience = apiName,
                                }, out var _);
                                _cache.Set(token, cacheEntry, new MemoryCacheEntryOptions()
                                    .SetSlidingExpiration(TimeSpan.FromSeconds(3600)));
                            }
                        }

                        await context.HttpContext.SignInAsync(DefaultAuthenticationScheme, cacheEntry);
                        context.HttpContext.User = cacheEntry;
                        return;
                    }
                    catch (SecurityTokenExpiredException ex)
                    {
                        _logger.LogWarning($"验证授权失败：{apiName} {issUrl} {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "验证授权失败：{apiName} {issUrl}", apiName, issUrl);
                    }
                }

                context.Result = new UnauthorizedResult();
            }
            else
            {
                if (result?.Principal != null)
                {
                    context.HttpContext.User = result.Principal;
                }
            }

            return;
        }
    }
}
