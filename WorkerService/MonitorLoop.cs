using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tranzaksyon.Database.AppDbContext;
using Tranzaksyon.Database.Data;
using Tranzaksyon.Database.Models;
using WorkerService.Taks;

namespace WorkerService
{
    public sealed class MonitorLoop(
    IBackgroundTaskQueue taskQueue,
    ILogger<MonitorLoop> logger,
    IHostApplicationLifetime applicationLifetime,
    IServiceProvider serviceProvider)
    {
        private readonly CancellationToken _cancellationToken = applicationLifetime.ApplicationStopping;

        public void StartMonitorLoop()
        {
            logger.LogInformation($"{nameof(MonitorAsync)} loop is starting.");

            // Run a console user input loop in a background thread
            Task.Run(async () => await MonitorAsync());
        }

        private async ValueTask MonitorAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<AppSqlServerContext>();

                        // Todo: Usar parametros para el tiempo de espera
                        DateTime past = DateTime.Now.Subtract(TimeSpan.FromMinutes(30));

                        var orders = context.Orders.OrderBy(o => o.UpdatedDate).Where(o => (o.ProcessedDate == null || o.ProcessedDate < past) && o.Status == OrderStates.Pendiente).Take(10);

                        foreach (Order order in orders)
                        {
                            try
                            {
                                OrderTask orderTask = new(order, taskQueue, logger, serviceProvider);
                                await orderTask.AddQueue();
                            }
                            catch (Exception ex)
                            {
                                logger.LogError("Hubo error con el order {order}. {error}", order.Id, ex.ToString());
                                context.OrderLogs.Add(new OrderLog
                                {
                                    Order = order,
                                    OrderId = order.Id,
                                    DateCreated = DateTime.Now,
                                    Error = ex.Message
                                });
                            }
                        }

                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Hubo error procesando los orders.");
                }

                // Esperar 5 segundos para recuperar de la base de datos otra vez
                Task.Delay(10000).Wait();
            }
        }


    }
}
