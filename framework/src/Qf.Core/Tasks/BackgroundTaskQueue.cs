using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.Core.Tasks
{
    public interface IBackgroundTaskQueue
    {
        public long Count { get; }
        void QueueBackgroundWorkItem(Func<IBackgroundTaskQueue, CancellationToken, Task> workItem);
        void QueueBackgroundWorkItem(Func<IBackgroundTaskQueue, CancellationToken, Task> workItem, double delaySeconds);
        Task<Func<IBackgroundTaskQueue, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue, IDisposable
    {
        private static ConcurrentQueue<Func<IBackgroundTaskQueue, CancellationToken, Task>> _workItems = new ConcurrentQueue<Func<IBackgroundTaskQueue, CancellationToken, Task>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(Func<IBackgroundTaskQueue, CancellationToken, Task> workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            _workItems.Enqueue(workItem);
            _signal.Release();
        }
        public long Count
        {
            get
            {
                return _workItems.Count;
            }
        }
        public void QueueBackgroundWorkItem(Func<IBackgroundTaskQueue, CancellationToken, Task> workItem, double delaySeconds)
        {
            Task.Delay(TimeSpan.FromSeconds(delaySeconds))
                .ContinueWith(t =>
                {
                    QueueBackgroundWorkItem(workItem);
                });
        }
        public async Task<Func<IBackgroundTaskQueue, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);
            return workItem;
        }
        private bool isDisposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;
            if (disposing)
            {

            }
            _signal.Dispose();
            isDisposed = true;
        }
    }
}
