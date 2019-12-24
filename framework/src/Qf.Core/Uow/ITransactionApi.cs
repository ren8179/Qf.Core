using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qf.Core.Uow
{
    public interface ITransactionApi : IDisposable
    {
        void Commit();

        Task CommitAsync();
    }
}
