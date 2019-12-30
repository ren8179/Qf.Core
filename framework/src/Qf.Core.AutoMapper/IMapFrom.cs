using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.AutoMapper
{
    public interface IMapFrom<in TSource>
    {
        void MapFrom(TSource source);
    }
}
