using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.AutoMapper
{
    public interface IMapTo<TDestination>
    {
        TDestination MapTo();

        void MapTo(TDestination destination);
    }
}
