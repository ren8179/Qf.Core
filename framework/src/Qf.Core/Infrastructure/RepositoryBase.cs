using Qf.Core.DependencyInjection;
using Qf.Core.Uow;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.Core.Infrastructure
{
    public abstract class RepositoryBase<TEntity,TKey> : IRepository<TEntity,TKey>,
        IServiceProviderAccessor,
        IUnitOfWorkEnabled,
        ITransientDependency
        where TEntity : IEntity<TKey>
    {
        public IServiceProvider ServiceProvider { get; set; }

        public ICancellationTokenProvider CancellationTokenProvider { get; set; }

        protected RepositoryBase()
        {
            CancellationTokenProvider = NullCancellationTokenProvider.Instance;
        }

        public abstract TEntity Add(TEntity model, bool autoSave = false);

        public virtual Task<TEntity> AddAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Add(entity, autoSave));
        }

        public abstract TEntity Update(TEntity model, bool autoSave = false);
        public virtual Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Update(entity, autoSave));
        }

        public abstract void Del(TEntity model, bool autoSave = false);
        public abstract Task DelAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default);
        public virtual Task DelAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            Del(entity, autoSave);
            return Task.CompletedTask;
        }
        public abstract Task DelAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default);
        protected virtual CancellationToken GetCancellationToken(CancellationToken prefferedValue = default)
        {
            return prefferedValue == default || prefferedValue == CancellationToken.None
                ? CancellationTokenProvider.Token
                : prefferedValue;
        }
        public abstract TEntity Get(TKey id);
        public abstract TEntity Get(Expression<Func<TEntity, bool>> predicate);
        public abstract Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);
        public abstract Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        public abstract Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        public abstract Task<long> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        public abstract void Complete();
        public abstract Task CompleteAsync(CancellationToken cancellationToken = default);
    }
}
