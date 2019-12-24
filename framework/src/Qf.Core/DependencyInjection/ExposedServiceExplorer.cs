using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Qf.Core.DependencyInjection
{
    public static class ExposedServiceExplorer
    {
        private static readonly ExposeServicesAttribute DefaultExposeServicesAttribute =
            new ExposeServicesAttribute
            {
                IncludeDefaults = true,
                IncludeSelf = true
            };

        public static List<Type> GetExposedServices(Type type)
        {
            return type
                .GetCustomAttributes()
                .OfType<IExposedServiceTypesProvider>()
                .DefaultIfEmpty(DefaultExposeServicesAttribute)
                .SelectMany(p => p.GetExposedServiceTypes(type))
                .ToList();
        }
    }
}
