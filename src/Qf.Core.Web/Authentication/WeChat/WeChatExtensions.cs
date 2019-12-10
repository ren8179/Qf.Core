using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qf.Core.Web.Authentication.WeChat
{
    public static class WeChatExtensions
    {
        public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder)
            => builder.AddWeChat(WeChatDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder, Action<WeChatOptions> configureOptions)
            => builder.AddWeChat(WeChatDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder, string authenticationScheme, Action<WeChatOptions> configureOptions)
            => builder.AddWeChat(authenticationScheme, WeChatDefaults.DisplayName, configureOptions);


        public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WeChatOptions> configureOptions)
        {

            builder.Services.TryAddTransient<ISecureDataFormat<AuthenticationProperties>>((provider) =>
            {
                var dataProtectionProvider = provider.GetRequiredService<IDataProtectionProvider>();
                var distributedCache = provider.GetRequiredService<IDistributedCache>();

                var dataProtector = dataProtectionProvider.CreateProtector(
                    typeof(WeChatHandler).FullName,
                    typeof(string).FullName, WeChatDefaults.AuthenticationScheme,
                    "v1");

                var dataFormat = new CachedPropertiesDataFormat(distributedCache, dataProtector);
                return dataFormat;
            });


            return builder.AddOAuth<WeChatOptions, WeChatHandler>(authenticationScheme,
                displayName, configureOptions);
        }



    }
}
