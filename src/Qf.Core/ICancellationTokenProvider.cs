using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Qf.Core
{
    public interface ICancellationTokenProvider
    {
        CancellationToken Token { get; }
    }
}
