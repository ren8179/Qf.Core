using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.Core.Uow
{
    public interface ISupportsRollback
    {
        void Rollback();

        Task RollbackAsync(CancellationToken cancellationToken);
    }
}
