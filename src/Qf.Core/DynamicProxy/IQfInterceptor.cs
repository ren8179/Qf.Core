using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qf.Core.DynamicProxy
{
    public interface IQfInterceptor
    {
        void Intercept(IQfMethodInvocation invocation);

        Task InterceptAsync(IQfMethodInvocation invocation);
    }
}
