using JetBrains.Annotations;
using Qf.Core.DependencyInjection;
using System;

namespace Qf.Core.Uow
{
    public interface IDatabaseApiContainer : IServiceProviderAccessor
    {
        [CanBeNull]
        IDatabaseApi FindDatabaseApi([NotNull] string key);

        void AddDatabaseApi([NotNull] string key, [NotNull] IDatabaseApi api);

        [NotNull]
        IDatabaseApi GetOrAddDatabaseApi([NotNull] string key, [NotNull] Func<IDatabaseApi> factory);
    }
}
