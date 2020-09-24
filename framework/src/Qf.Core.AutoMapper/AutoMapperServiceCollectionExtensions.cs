using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Qf.Core.DependencyInjection;
using Qf.Core.Helper;

namespace Qf.Core.AutoMapper
{
    public static class AutoMapperServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapperObjectMapper(this IServiceCollection services)
        {
            services.OnExposing(onServiceExposingContext =>
            {
                //Register types for IObjectMapper<TSource, TDestination> if implements
                onServiceExposingContext.ExposedTypes.AddRange(
                    ReflectionHelper.GetImplementedGenericTypes(
                        onServiceExposingContext.ImplementationType,
                        typeof(IObjectMapper<,>)
                    )
                );
            });
            services.AddTransient(typeof(IObjectMapper<>),typeof(DefaultObjectMapper<>));
            services.Replace(
                ServiceDescriptor.Transient<IAutoObjectMappingProvider, AutoMapperAutoObjectMappingProvider>()
            );
            var mapperAccessor = new MapperAccessor();
            services.AddSingleton<IMapperAccessor>(_ => mapperAccessor);
            services.AddSingleton<MapperAccessor>(_ => mapperAccessor);
            return services;
        }
    }
}
