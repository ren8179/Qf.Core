using JetBrains.Annotations;
using Qf.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.DependencyInjection
{
    public class OnServiceExposingContext : IOnServiceExposingContext
    {
        public Type ImplementationType { get; }

        public List<Type> ExposedTypes { get; }

        public OnServiceExposingContext([NotNull] Type implementationType, List<Type> exposedTypes)
        {
            ImplementationType = Check.NotNull(implementationType, nameof(implementationType));
            ExposedTypes = Check.NotNull(exposedTypes, nameof(exposedTypes));
        }
    }
}
