namespace Qf.Core.Web.Authentication.AliPay
{
    public static class AliPayDefaults
    {
        public const string AuthenticationScheme = "AliPay";

        public static readonly string DisplayName = "AliPay";

        /// <summary>
        /// URL 拼接与 scope
        /// </summary>
        public static readonly string AuthorizationEndpoint = "https://openauth.alipay.com/oauth2/publicAppAuthorize.htm";
        /// <summary>
        /// 第二步，用户允许授权后，通过返回的code换取access_token地址
        /// </summary>
        public static readonly string TokenEndpoint = "https://openapi.alipay.com/gateway.do";

        /// <summary>
        /// 第三步，使用access_token获取用户个人信息地址
        /// </summary>
        public static readonly string UserInformationEndpoint = "https://openapi.alipay.com/gateway.do";
    }
}
