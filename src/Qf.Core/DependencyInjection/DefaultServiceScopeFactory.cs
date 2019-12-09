using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.DependencyInjection
{
    [ExposeServices(
        typeof(IHybridServiceScopeFactory),
        typeof(DefaultServiceScopeFactory)
        )]
    public class DefaultServiceScopeFactory : IHybridServiceScopeFactory, ITransientDependency
    {
        protected IServiceScopeFactory Factory { get; }

        public DefaultServiceScopeFactory(IServiceScopeFactory factory)
        {
            Factory = factory;
        }

        public IServiceScope CreateScope()
        {
            return Factory.CreateScope();
        }
    }
}
