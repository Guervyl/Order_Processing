using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService
{
    public interface IBackgroundTaskEnQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(
        Func<CancellationToken, ValueTask> workItem);
    }
}
