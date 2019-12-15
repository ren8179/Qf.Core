using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qf.Core.DelayQueues
{
    public class DelayQueue<T> where T : DelayQueueParam
    {
        private List<T>[] queue;

        private int currentIndex = 1;

        public DelayQueue(int length)
        {
            queue = new List<T>[length];
        }

        public void Insert(T item, int time)
        {
            item.CycleNum = time / queue.Length;
            item.Slot = (time + currentIndex) % queue.Length;
            //加入到延时队列中
            if (queue[item.Slot] == null)
            {
                queue[item.Slot] = new List<T>();
            }
            queue[item.Slot].Add(item);
        }
        public void Remove(T item)
        {
            if (queue[item.Slot] != null)
            {
                queue[item.Slot].Remove(item);
            }
        }

        public void Read()
        {
            if (queue.Length >= currentIndex)
            {
                var list = queue[currentIndex - 1];
                if (list != null)
                {
                    List<T> target = new List<T>();
                    foreach (var item in list)
                    {
                        if (item.CycleNum == 0)
                        {
                            //在本轮命中，用单独线程去执行业务操作
                            Task.Run(() => {
                                try
                                {
                                    item.Callback(item);
                                }
                                finally { }
                            });
                            target.Add(item);
                        }
                        else
                        {
                            //等下一轮
                            item.CycleNum--;
                            System.Diagnostics.Debug.WriteLine($"@@@@@索引：{item.Slot}，剩余：{item.CycleNum}");
                        }
                    }
                    //把已过期的移除掉
                    foreach (var item in target)
                    {
                        list.Remove(item);
                    }
                }
                currentIndex++;
                //下一遍从头开始
                if (currentIndex > queue.Length)
                {
                    currentIndex = 1;
                }
            }
        }
    }
}
