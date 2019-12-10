using Microsoft.EntityFrameworkCore;
using Qf.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.EFCore.Repositories
{
    public interface IEfCoreRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
    {
        DbContext DbContext { get; }

        DbSet<TEntity> DbSet { get; }
    }
}
