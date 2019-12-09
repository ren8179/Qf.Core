using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qf.Core.DynamicProxy
{
    public interface IInterceptor
    {
        void Intercept(IMethodInvocation invocation);

        Task InterceptAsync(IMethodInvocation invocation);
    }
}
