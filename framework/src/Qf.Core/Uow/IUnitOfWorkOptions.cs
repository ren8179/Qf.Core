using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Qf.Core.Uow
{
    public interface IUnitOfWorkOptions
    {
        bool IsTransactional { get; }

        IsolationLevel? IsolationLevel { get; }

        TimeSpan? Timeout { get; }
    }
}
