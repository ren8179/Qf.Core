using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qf.Core.DynamicProxy
{
    public abstract class Interceptor : IInterceptor
    {
        public abstract void Intercept(IMethodInvocation invocation);

        public virtual Task InterceptAsync(IMethodInvocation invocation)
        {
            Intercept(invocation);
            return Task.CompletedTask;
        }
    }
}
