using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.Core.Uow
{
    public interface IUnitOfWork : IDatabaseApiContainer, ITransactionApiContainer, IDisposable
    {
        Guid Id { get; }

        event EventHandler<UnitOfWorkFailedEventArgs> Failed;

        event EventHandler<UnitOfWorkEventArgs> Disposed;

        IUnitOfWorkOptions Options { get; }

        IUnitOfWork Outer { get; }

        bool IsReserved { get; }

        bool IsDisposed { get; }

        bool IsCompleted { get; }

        string ReservationName { get; }

        void SetOuter([CanBeNull] IUnitOfWork outer);

        void Initialize([NotNull] UnitOfWorkOptions options);

        void Reserve([NotNull] string reservationName);

        void SaveChanges();

        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        void Complete();

        Task CompleteAsync(CancellationToken cancellationToken = default);

        void Rollback();

        Task RollbackAsync(CancellationToken cancellationToken = default);

        void OnCompleted(Func<Task> handler);
    }
}
