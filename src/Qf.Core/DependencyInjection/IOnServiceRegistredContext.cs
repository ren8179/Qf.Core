using Qf.Core.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.DependencyInjection
{
    public interface IOnServiceRegistredContext
    {
        ITypeList<IInterceptor> Interceptors { get; }

        Type ImplementationType { get; }
    }
}
