using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qf.Core.AutoMapper;
using System;

namespace Qf.Core.Web.Extension
{
    public static class AutoMapperExtensions
    {
        public static void UseQfAutoMapper(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<IOptions<AutoMapperOptions>>().Value;
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();

                var mapperConfiguration = new MapperConfiguration(mapperConfigurationExpression =>
                {
                    foreach (var configurator in options.Configurators)
                    {
                        configurator(new AutoMapperConfigurationContext(mapperConfigurationExpression, scope.ServiceProvider));
                    }
                }, loggerFactory);

                mapperConfiguration.AssertConfigurationIsValid();

                scope.ServiceProvider.GetRequiredService<MapperAccessor>().Mapper = mapperConfiguration.CreateMapper();
            }
        }
    }
}
