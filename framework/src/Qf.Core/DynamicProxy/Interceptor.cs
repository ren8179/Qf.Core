using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qf.Core.DynamicProxy
{
    public abstract class QfInterceptor : IQfInterceptor
    {
        public abstract void Intercept(IQfMethodInvocation invocation);

        public virtual Task InterceptAsync(IQfMethodInvocation invocation)
        {
            Intercept(invocation);
            return Task.CompletedTask;
        }
    }
}
