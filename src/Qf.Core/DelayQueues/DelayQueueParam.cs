using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.DelayQueues
{
    public class DelayQueueParam
    {
        internal int Slot { get; set; }

        internal int CycleNum { get; set; }

        public Action<object> Callback { get; set; }
    }
}
