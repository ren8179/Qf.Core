using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Qf.Core.DependencyInjection;
using System;

namespace Qf.Core.EFCore.DependencyInjection
{
    public static class EfCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddQfDbContext<TDbContext>(
            this IServiceCollection services,
            Action<ICommonDbContextRegistrationOptionsBuilder> optionsBuilder = null)
            where TDbContext : QfDbContext<TDbContext>
        {
            services.AddMemoryCache();

            var options = new DbContextRegistrationOptions(typeof(TDbContext), services);
            optionsBuilder?.Invoke(options);

            services.TryAddTransient(DbContextOptionsFactory.Create<TDbContext>);

            foreach (var dbContextType in options.ReplacedDbContextTypes)
            {
                services.Replace(ServiceDescriptor.Transient(dbContextType, typeof(TDbContext)));
            }

            new EfCoreRepositoryRegistrar(options).AddRepositories();

            return services;
        }
    }
}
