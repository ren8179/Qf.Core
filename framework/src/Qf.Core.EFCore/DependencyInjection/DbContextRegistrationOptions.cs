using Microsoft.Extensions.DependencyInjection;
using Qf.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.EFCore.DependencyInjection
{
    public class DbContextRegistrationOptions : CommonDbContextRegistrationOptions, ICommonDbContextRegistrationOptionsBuilder
    {

        public DbContextRegistrationOptions(Type originalDbContextType, IServiceCollection services)
            : base(originalDbContextType, services)
        {
        }
    }
}
