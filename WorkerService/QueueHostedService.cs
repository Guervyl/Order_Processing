using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService
{
    public sealed class QueuedHostedService(
        IBackgroundTaskQueue taskQueue,
        ILogger<QueuedHostedService> logger,
        IConfiguration configuration) : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("""
            {Name} is running.
            """,
                nameof(QueuedHostedService));

            return ProcessTaskQueueAsync(stoppingToken);
        }

        private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Task[] tasks = [];
                    //Todo: Controlar si es null o no es int
                    int numJobs = int.Parse(configuration["tareasMax"]);

                    for (int i = 0; i < numJobs; i++)
                    {
                        Func<CancellationToken, ValueTask>? workItem =
                            await taskQueue.DequeueAsync(stoppingToken);

                        tasks.Append(Task.Run(async () => await workItem(stoppingToken)));
                    }

                    Task.WaitAny(tasks);
                }
                catch (OperationCanceledException)
                {
                    // Gestionar cuando estan cancelados
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred executing task work item.");
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation(
                $"{nameof(QueuedHostedService)} is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
