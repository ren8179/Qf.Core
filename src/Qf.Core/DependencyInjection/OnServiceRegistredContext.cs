using JetBrains.Annotations;
using Qf.Core.DynamicProxy;
using Qf.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.DependencyInjection
{
    public class OnServiceRegistredContext : IOnServiceRegistredContext
    {
        public virtual ITypeList<IInterceptor> Interceptors { get; }

        public virtual Type ServiceType { get; }

        public virtual Type ImplementationType { get; }

        public OnServiceRegistredContext(Type serviceType, [NotNull] Type implementationType)
        {
            ServiceType = Check.NotNull(serviceType, nameof(serviceType));
            ImplementationType = Check.NotNull(implementationType, nameof(implementationType));

            Interceptors = new TypeList<IInterceptor>();
        }
    }
}
