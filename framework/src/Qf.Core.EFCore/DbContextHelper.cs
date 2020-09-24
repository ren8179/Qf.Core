using Microsoft.EntityFrameworkCore;
using Qf.Core.Helper;
using Qf.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Qf.Core.EFCore
{
    internal static class DbContextHelper
    {
        public static IEnumerable<Type> GetEntityTypes(Type dbContextType)
        {
            return
                from property in dbContextType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>)) &&
                    typeof(IEntity).IsAssignableFrom(property.PropertyType.GenericTypeArguments[0])
                select property.PropertyType.GenericTypeArguments[0];
        }
    }
}
