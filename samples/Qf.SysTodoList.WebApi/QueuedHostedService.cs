using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Qf.Core.DelayQueues;
using Qf.Core.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.SysTodoList.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public class QueuedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IBackgroundTask> _tasks;
        private readonly CancellationTokenSource _shutdown = new CancellationTokenSource();

        public QueuedHostedService(IServiceProvider services)
        {
            _tasks = services.GetServices<IBackgroundTask>();
            _logger = services.GetRequiredService<ILogger<QueuedHostedService>>(); ;
            TaskQueue = services.GetRequiredService<IBackgroundTaskQueue>();
        }

        /// <summary>
        /// Gets 任务队列
        /// </summary>
        public IBackgroundTaskQueue TaskQueue { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Background Service is starting.");
            Begin();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Background Service is stopping.");
            _shutdown.Cancel();
            return Task.CompletedTask;
        }
        /// <summary>
        /// 开始
        /// </summary>
        public void Begin()
        {
            if (_tasks != null && _tasks.Any())
            {
                _logger.LogInformation($"发现{_tasks.Count()}个后台服务.");
                foreach (var item in _tasks)
                {
                    TaskQueue.QueueBackgroundWorkItem(item.RunAsync);
                }
                Task.Run(BackgroundProceessing).ConfigureAwait(false);
            }

            Task.Delay(TimeSpan.FromSeconds(30)).ContinueWith(HourTimingProceessing).ConfigureAwait(false);
        }

        /// <summary>
        /// 后台线程
        /// </summary>
        /// <returns></returns>
        private async Task BackgroundProceessing()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(_shutdown.Token).ConfigureAwait(false);
                if (workItem == null) return;
                try
                {
                    await workItem(TaskQueue, _shutdown.Token).ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromSeconds(10), _shutdown.Token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"执行任务出错 {nameof(workItem)} 1800秒之后重试");
                    TaskQueue.QueueBackgroundWorkItem(workItem, 1800);
                }
            }
        }

        /// <summary>
        /// 定时线程,小时
        /// </summary>
        /// <returns></returns>
        private async Task HourTimingProceessing(object task)
        {
            while (!_shutdown.IsCancellationRequested)
            {
                try
                {
                    DelayQueueManager<DelayQueueParam>.Read();
                    await Task.Delay(TimeSpan.FromHours(1), _shutdown.Token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"读取时间轮出错 1800秒之后重试");
                    await Task.Delay(TimeSpan.FromSeconds(1800), _shutdown.Token);
                }
            }
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
            _shutdown.Dispose();
            isDisposed = true;
        }
    }
}
