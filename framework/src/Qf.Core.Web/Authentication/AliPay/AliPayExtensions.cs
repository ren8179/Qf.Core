using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Qf.Core.Web.Authentication.AliPay
{
    public static class AliPayExtensions
    {
        public static AuthenticationBuilder AddAliPay(this AuthenticationBuilder builder)
            => builder.AddAliPay(AliPayDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddAliPay(this AuthenticationBuilder builder, Action<AliPayOptions> configureOptions)
            => builder.AddAliPay(AliPayDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddAliPay(this AuthenticationBuilder builder, string authenticationScheme, Action<AliPayOptions> configureOptions)
            => builder.AddAliPay(authenticationScheme, AliPayDefaults.DisplayName, configureOptions);


        public static AuthenticationBuilder AddAliPay(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<AliPayOptions> configureOptions)
        {
            builder.Services.TryAddTransient<ISecureDataFormat<AuthenticationProperties>>((provider) =>
            {
                var dataProtectionProvider = provider.GetRequiredService<IDataProtectionProvider>();
                var distributedCache = provider.GetRequiredService<IDistributedCache>();

                var dataProtector = dataProtectionProvider.CreateProtector(
                    typeof(AliPayHandler).FullName,
                    typeof(string).FullName, AliPayDefaults.AuthenticationScheme,
                    "v1");

                var dataFormat = new CachedPropertiesDataFormat(distributedCache, dataProtector);
                return dataFormat;
            });

            return builder.AddOAuth<AliPayOptions, AliPayHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
