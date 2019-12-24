using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Qf.Core
{
    public class NullCancellationTokenProvider : ICancellationTokenProvider
    {
        public static NullCancellationTokenProvider Instance { get; } = new NullCancellationTokenProvider();

        public CancellationToken Token { get; } = CancellationToken.None;

        private NullCancellationTokenProvider()
        {

        }
    }
}
