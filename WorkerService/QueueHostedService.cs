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
        List<Task> tasks = new List<Task>();
        int numJobs;

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            if (!int.TryParse(configuration["tareasMax"], out numJobs))
            {
                throw new ArgumentException("tareasMax no es un int");
            }

            return base.StartAsync(cancellationToken);
        }

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
                    Func<CancellationToken, ValueTask>? workItem =
                        await taskQueue.DequeueAsync(stoppingToken);

                    tasks.Add(Task.Run(async () => await workItem(stoppingToken)));

                    if (tasks.Count >= numJobs)
                    {
                        await Task.WhenAll(tasks);
                        tasks.Clear();
                    }
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
