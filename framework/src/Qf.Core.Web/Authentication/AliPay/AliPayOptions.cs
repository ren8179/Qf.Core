using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using static System.Formats.Asn1.AsnWriter;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;

namespace Qf.Core.Web.Authentication.AliPay
{
    public class AliPayOptions : OAuthOptions
    {
        public static string UserInfoScope = "auth_userinfo";
        public static string LoginScope = "auth_base";

        public AliPayOptions()
        {
            CallbackPath = new PathString("/signin-alipay");
            AuthorizationEndpoint = AliPayDefaults.AuthorizationEndpoint;
            TokenEndpoint = AliPayDefaults.TokenEndpoint;
            UserInformationEndpoint = AliPayDefaults.UserInformationEndpoint;

            //Scope 表示应用授权作用域。
            //目前只支持 auth_userinfo 和 auth_base 两个值
            //Scope.Add(UserInfoScope);
            Scope.Add(LoginScope);

            //除了openid外，其余的都可能为空，因为微信获取用户信息是有单独权限的
            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "user_id");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "nick_name");
            ClaimActions.MapJsonKey(ClaimTypes.Uri, "avatar");

            IsAliPayBrowser = (r) => r.Headers[HeaderNames.UserAgent].ToString().ToLower().Contains("alipay");
        }

        /// <summary>
        /// 应用唯一标识
        /// </summary>
        public string AppId
        {
            get { return ClientId; }
            set { ClientId = value; }
        }
        /// <summary>
        /// 应用私钥
        /// </summary>
        public string PrivateKey { get; set; }
        /// <summary>
        /// 应用公钥
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// 回调URL
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// 是否是云支付内置浏览器
        /// </summary>
        public Func<HttpRequest, bool> IsAliPayBrowser { get; set; }

        public bool UseCachedStateDataFormat { get; set; } = false;

    }
}
