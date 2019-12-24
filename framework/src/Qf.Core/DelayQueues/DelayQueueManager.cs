using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.DelayQueues
{
    public static class DelayQueueManager<T> where T : DelayQueueParam
    {
        private static DelayQueue<T> _queue = new DelayQueue<T>(60);

        public static void Insert(T plan, int time)
        {
            _queue.Insert(plan, time);
        }
        public static void Read()
        {
            _queue.Read();
        }
        public static void Remove(T item)
        {
            _queue.Remove(item);
        }
    }
}
