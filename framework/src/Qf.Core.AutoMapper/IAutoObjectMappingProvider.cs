using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.AutoMapper
{
    public interface IAutoObjectMappingProvider
    {
        TDestination Map<TSource, TDestination>(object source);

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }
    public interface IAutoObjectMappingProvider<TContext> : IAutoObjectMappingProvider
    {

    }
}
