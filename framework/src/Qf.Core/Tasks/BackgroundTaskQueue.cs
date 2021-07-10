using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.Core.Tasks
{
    /// <summary>
    /// 后台任务队列
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// 获取队列中的任务对象数量
        /// </summary>
        public long Count { get; }
        /// <summary>
        /// 添加任务对象至队列末尾
        /// </summary>
        void QueueBackgroundWorkItem(Func<IBackgroundTaskQueue, CancellationToken, Task> workItem);
        /// <summary>
        /// 延迟添加任务对象至队列末尾 延迟单位:秒
        /// </summary>
        void QueueBackgroundWorkItem(Func<IBackgroundTaskQueue, CancellationToken, Task> workItem, double delaySeconds);
        /// <summary>
        /// 移除并返回队列首位的任务对象
        /// </summary>
        Task<Func<IBackgroundTaskQueue, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
        /// <summary>
        /// 清空队列
        /// </summary>
        void Clear();
    }
    /// <summary>
    /// 后台任务队列
    /// </summary>
    public class BackgroundTaskQueue : IBackgroundTaskQueue, IDisposable
    {
        private static ConcurrentQueue<Func<IBackgroundTaskQueue, CancellationToken, Task>> _workItems = new ConcurrentQueue<Func<IBackgroundTaskQueue, CancellationToken, Task>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        /// <summary>
        /// 添加任务对象至队列末尾
        /// </summary>
        public void QueueBackgroundWorkItem(Func<IBackgroundTaskQueue, CancellationToken, Task> workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            _workItems.Enqueue(workItem);
            _signal.Release();
        }
        /// <summary>
        /// 获取队列中的任务对象数量
        /// </summary>
        public long Count
        {
            get
            {
                return _workItems.Count;
            }
        }
        /// <summary>
        /// 延迟添加任务对象至队列末尾 延迟单位:秒
        /// </summary>
        public void QueueBackgroundWorkItem(Func<IBackgroundTaskQueue, CancellationToken, Task> workItem, double delaySeconds)
        {
            Task.Delay(TimeSpan.FromSeconds(delaySeconds))
                .ContinueWith(t =>
                {
                    QueueBackgroundWorkItem(workItem);
                });
        }
        /// <summary>
        /// 移除并返回队列首位的任务对象
        /// </summary>
        public async Task<Func<IBackgroundTaskQueue, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            if (_workItems.IsEmpty) return null;
            _workItems.TryDequeue(out var workItem);
            return workItem;
        }
        /// <summary>
        /// 清空队列
        /// </summary>
        public void Clear()
        {
            _workItems.Clear();
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
