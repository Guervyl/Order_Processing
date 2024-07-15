using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService
{
    public interface IBackgroundTaskQueue : IBackgroundTaskEnQueue
    {
        ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
            CancellationToken cancellationToken);
    }
}
