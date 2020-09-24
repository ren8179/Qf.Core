using AutoMapper;
using System;

namespace Qf.Core.AutoMapper
{
    public class AutoMapperConfigurationContext : IAutoMapperConfigurationContext
    {
        public IMapperConfigurationExpression MapperConfiguration { get; }
        public IServiceProvider ServiceProvider { get; }

        public AutoMapperConfigurationContext(
            IMapperConfigurationExpression mapperConfigurationExpression,
            IServiceProvider serviceProvider)
        {
            MapperConfiguration = mapperConfigurationExpression;
            ServiceProvider = serviceProvider;
        }
    }
}
