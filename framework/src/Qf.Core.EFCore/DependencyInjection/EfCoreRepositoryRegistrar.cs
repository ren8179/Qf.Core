using Qf.Core.DependencyInjection;
using Qf.Core.EFCore.Repositories;
using Qf.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.EFCore.DependencyInjection
{
    public class EfCoreRepositoryRegistrar : RepositoryRegistrarBase<CommonDbContextRegistrationOptions>
    {
        public EfCoreRepositoryRegistrar(CommonDbContextRegistrationOptions options)
            : base(options)
        {

        }

        protected override IEnumerable<Type> GetEntityTypes(Type dbContextType)
        {
            return DbContextHelper.GetEntityTypes(dbContextType);
        }


        protected override Type GetRepositoryType(Type dbContextType, Type entityType, Type primaryKeyType)
        {
            return typeof(EfCoreRepository<,,>).MakeGenericType(dbContextType, entityType, primaryKeyType);
        }
    }
}
