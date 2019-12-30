using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

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
