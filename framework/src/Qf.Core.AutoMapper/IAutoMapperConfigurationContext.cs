using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.AutoMapper
{
    public interface IAutoMapperConfigurationContext
    {
        IMapperConfigurationExpression MapperConfiguration { get; }

        IServiceProvider ServiceProvider { get; }
    }
}
