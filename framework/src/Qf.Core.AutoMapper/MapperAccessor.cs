using AutoMapper;

namespace Qf.Core.AutoMapper
{
    public interface IMapperAccessor
    {
        IMapper Mapper { get; }
    }
    public class MapperAccessor : IMapperAccessor
    {
        public IMapper Mapper { get; set; }
    }
}
