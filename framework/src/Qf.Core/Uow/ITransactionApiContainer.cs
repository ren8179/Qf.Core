using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.Uow
{
    public interface ITransactionApiContainer
    {
        [CanBeNull]
        ITransactionApi FindTransactionApi([NotNull] string key);

        void AddTransactionApi([NotNull] string key, [NotNull] ITransactionApi api);

        [NotNull]
        ITransactionApi GetOrAddTransactionApi([NotNull] string key, [NotNull] Func<ITransactionApi> factory);
    }
}
