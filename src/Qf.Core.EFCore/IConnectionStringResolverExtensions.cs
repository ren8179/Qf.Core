using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.EFCore
{
    public static class ConnectionStringResolverExtensions
    {
        public static string Resolve<T>(this IConnectionStringResolver resolver)
        {
            return resolver.Resolve(ConnectionStringNameAttribute.GetConnStringName<T>());
        }
    }
}
