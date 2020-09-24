using AutoMapper;
using System;

namespace Qf.Core.AutoMapper
{
    public interface IAutoMapperConfigurationContext
    {
        IMapperConfigurationExpression MapperConfiguration { get; }

        IServiceProvider ServiceProvider { get; }
    }
}
