using System.Threading;

namespace Qf.Core
{
    public interface ICancellationTokenProvider
    {
        CancellationToken Token { get; }
    }
}
