using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.Core.Tasks
{
    /// <summary>
    /// 后台任务
    /// </summary>
    public interface IBackgroundTask
    {
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="taskQueue"></param>
        /// <param name="cancellationToken"></param>
        Task RunAsync(IBackgroundTaskQueue taskQueue, CancellationToken cancellationToken);
    }
}
