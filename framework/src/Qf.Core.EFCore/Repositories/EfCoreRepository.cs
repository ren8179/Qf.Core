using Microsoft.EntityFrameworkCore;
using Qf.Core.Extensions;
using Qf.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.Core.EFCore.Repositories
{
    public class EfCoreRepository<TDbContext, TEntity, TKey> : RepositoryBase<TEntity, TKey>, IEfCoreRepository<TEntity, TKey>
        where TDbContext : IEfCoreDbContext
        where TEntity : class, IEntity<TKey>
    {
        public virtual DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

        DbContext IEfCoreRepository<TEntity, TKey>.DbContext => DbContext.As<DbContext>();

        protected virtual TDbContext DbContext => _dbContextProvider.GetDbContext();

        private readonly IDbContextProvider<TDbContext> _dbContextProvider;
        public EfCoreRepository(IDbContextProvider<TDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public override TEntity Add(TEntity entity, bool autoSave = false)
        {
            var savedEntity = DbSet.Add(entity).Entity;

            if (autoSave)
            {
                DbContext.SaveChanges();
            }

            return savedEntity;
        }
        public override async Task<TEntity> AddAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var savedEntity = DbSet.Add(entity).Entity;

            if (autoSave)
            {
                await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
            }

            return savedEntity;
        }

        public override TEntity Update(TEntity entity, bool autoSave = false)
        {
            DbContext.Attach(entity);

            var updatedEntity = DbContext.Update(entity).Entity;

            if (autoSave)
            {
                DbContext.SaveChanges();
            }

            return updatedEntity;
        }
        public override async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            DbContext.Attach(entity);

            var updatedEntity = DbContext.Update(entity).Entity;

            if (autoSave)
            {
                await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
            }

            return updatedEntity;
        }

        public override void Del(TEntity entity, bool autoSave = false)
        {
            DbSet.Remove(entity);

            if (autoSave)
            {
                DbContext.SaveChanges();
            }
        }
        public override async Task DelAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var entity = await DbSet.FindAsync(new object[] { id }, GetCancellationToken(cancellationToken));
            if (entity == null)
            {
                return;
            }
            DbSet.Remove(entity);

            if (autoSave)
            {
                await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
            }
        }
        public override async Task DelAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            DbSet.Remove(entity);

            if (autoSave)
            {
                await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
            }
        }

        public override async Task DelAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var entity = await DbSet.AsQueryable().FirstOrDefaultAsync(predicate, GetCancellationToken(cancellationToken));
            DbSet.Remove(entity);

            if (autoSave)
            {
                await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
            }
        }
        public override TEntity Get(TKey id)
        {
            var entity = DbSet.Find(new object[] { id });
            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id);
            }

            return entity;
        }
        public override TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsQueryable().FirstOrDefault(predicate);
        }
        public override async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await DbSet.FindAsync(new object[] { id }, GetCancellationToken(cancellationToken));
            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id);
            }

            return entity;
        }
        public override async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.AsQueryable().FirstOrDefaultAsync(predicate, GetCancellationToken(cancellationToken));
        }
        public override async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.AsQueryable().Where(predicate).ToListAsync(GetCancellationToken(cancellationToken));
        }
        public override long GetCount(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.LongCount(predicate);
        }
        public override async Task<long> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.LongCountAsync(predicate, GetCancellationToken(cancellationToken));
        }
        public override void Complete()
        {
            DbContext.SaveChanges();
        }
        public override async Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
    }
}
