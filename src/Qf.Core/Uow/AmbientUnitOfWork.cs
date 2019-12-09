using Qf.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Qf.Core.Uow
{
    [ExposeServices(typeof(IAmbientUnitOfWork), typeof(IUnitOfWorkAccessor))]
    public class AmbientUnitOfWork : IAmbientUnitOfWork, ISingletonDependency
    {
        public IUnitOfWork UnitOfWork => _currentUow.Value;

        private readonly AsyncLocal<IUnitOfWork> _currentUow;

        public AmbientUnitOfWork()
        {
            _currentUow = new AsyncLocal<IUnitOfWork>();
        }

        public void SetUnitOfWork(IUnitOfWork unitOfWork)
        {
            _currentUow.Value = unitOfWork;
        }
    }
}
