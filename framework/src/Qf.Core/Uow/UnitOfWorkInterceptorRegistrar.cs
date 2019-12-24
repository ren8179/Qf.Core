using Qf.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Qf.Core.Uow
{
    public static class UnitOfWorkInterceptorRegistrar
    {
        public static void RegisterIfNeeded(IOnServiceRegistredContext context)
        {
            if (UnitOfWorkHelper.IsUnitOfWorkType(context.ImplementationType.GetTypeInfo()))
            {
                context.Interceptors.TryAdd<UnitOfWorkInterceptor>();
            }
        }
    }
}
