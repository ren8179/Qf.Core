using Castle.DynamicProxy;
using Qf.Core.Helper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qf.Core.DynamicProxy.Castle
{
    public class CastleInterceptorAdapter<TInterceptor> : IInterceptor
        where TInterceptor : IQfInterceptor
    {
        private static readonly MethodInfo MethodExecuteWithoutReturnValueAsync =
            typeof(CastleInterceptorAdapter<TInterceptor>)
                .GetMethod(
                    nameof(ExecuteWithoutReturnValueAsync),
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

        private static readonly MethodInfo MethodExecuteWithReturnValueAsync =
            typeof(CastleInterceptorAdapter<TInterceptor>)
                .GetMethod(
                    nameof(ExecuteWithReturnValueAsync),
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

        private readonly TInterceptor _interceptor;

        public CastleInterceptorAdapter(TInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        public void Intercept(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();

            var method = invocation.MethodInvocationTarget ?? invocation.Method;

            if (method.IsAsync())
            {
                InterceptAsyncMethod(invocation, proceedInfo);
            }
            else
            {
                InterceptSyncMethod(invocation, proceedInfo);
            }
        }

        private void InterceptSyncMethod(IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            _interceptor.Intercept(new CastleMethodInvocationAdapter(invocation, proceedInfo));
        }

        private void InterceptAsyncMethod(IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            if (invocation.Method.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = MethodExecuteWithoutReturnValueAsync
                    .Invoke(this, new object[] { invocation, proceedInfo });
            }
            else
            {
                invocation.ReturnValue = MethodExecuteWithReturnValueAsync
                    .MakeGenericMethod(invocation.Method.ReturnType.GenericTypeArguments[0])
                    .Invoke(this, new object[] { invocation, proceedInfo });
            }
        }

        private async Task ExecuteWithoutReturnValueAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            await Task.Yield();

            await _interceptor.InterceptAsync(
                new CastleMethodInvocationAdapter(invocation, proceedInfo)
            );
        }

        private async Task<T> ExecuteWithReturnValueAsync<T>(IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            await Task.Yield();

            await _interceptor.InterceptAsync(
                new CastleMethodInvocationAdapter(invocation, proceedInfo)
            );

            return await (Task<T>)invocation.ReturnValue;
        }
    }
}
