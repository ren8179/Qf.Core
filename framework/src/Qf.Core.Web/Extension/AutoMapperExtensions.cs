using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Qf.Core.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qf.Core.Web.Extension
{
    public static class AutoMapperExtensions
    {
        public static void UseQfAutoMapper(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<IOptions<AutoMapperOptions>>().Value;

                var mapperConfiguration = new MapperConfiguration(mapperConfigurationExpression =>
                {
                    foreach (var configurator in options.Configurators)
                    {
                        configurator(new AutoMapperConfigurationContext(mapperConfigurationExpression, scope.ServiceProvider));
                    }
                });
                foreach (var profileType in options.ValidatingProfiles)
                {
                    mapperConfiguration.AssertConfigurationIsValid(((Profile)Activator.CreateInstance(profileType)).ProfileName);
                }

                scope.ServiceProvider.GetRequiredService<MapperAccessor>().Mapper = mapperConfiguration.CreateMapper();
            }
        }
    }
}
