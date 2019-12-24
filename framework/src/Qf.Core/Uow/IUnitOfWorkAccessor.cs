using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.Uow
{
    public interface IUnitOfWorkAccessor
    {
        [CanBeNull]
        IUnitOfWork UnitOfWork { get; }

        void SetUnitOfWork([CanBeNull] IUnitOfWork unitOfWork);
    }
}
