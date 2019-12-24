using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.DependencyInjection
{
    public interface IOnServiceExposingContext
    {
        Type ImplementationType { get; }

        List<Type> ExposedTypes { get; }
    }
}
