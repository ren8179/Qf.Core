using Qf.Core.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.DependencyInjection
{
    public interface IOnServiceRegistredContext
    {
        ITypeList<IQfInterceptor> Interceptors { get; }

        Type ImplementationType { get; }
    }
}
