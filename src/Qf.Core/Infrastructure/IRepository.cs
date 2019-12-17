using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.Core.Infrastructure
{
    public interface IRepository
    {
    }
    public interface IRepository<TEntity, TKey> : IRepository where TEntity : IEntity<TKey>
    {
        TEntity Add([NotNull]TEntity model, bool autoSave = false);
        Task<TEntity> AddAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
        TEntity Update([NotNull]TEntity model, bool autoSave = false);
        Task<TEntity> UpdateAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
        void Del([NotNull]TEntity model, bool autoSave = false);
        Task DelAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
        Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<long> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
