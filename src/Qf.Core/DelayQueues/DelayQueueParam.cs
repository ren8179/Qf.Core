using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qf.Core.DelayQueues
{
    public class DelayQueueParam
    {
        internal int Slot { get; set; }

        internal int CycleNum { get; set; }

        public Func<object, Task> Callback { get; set; }
    }
}
