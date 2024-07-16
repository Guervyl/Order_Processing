using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tranzaksyon.Database.AppDbContext;
using Tranzaksyon.Database.Data;
using Tranzaksyon.Database.Models;

namespace WorkerService.Taks
{
    public class OrderTask(Order order, IBackgroundTaskQueue taskQueue, ILogger<MonitorLoop> logger, IServiceProvider serviceProvider)
    {
        public async Task AddQueue()
        {
            logger.LogInformation($"{nameof(OrderTask)} loop iniciado por el pedido {order.Id}.");
            // Run a console user input loop in a background thread
            await taskQueue.QueueBackgroundWorkItemAsync(BuildWorkItemAsync);

            order.ProcessedDate = DateTime.Now;
        }

        private async ValueTask BuildWorkItemAsync(CancellationToken token)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppSqlServerContext>();
                    Order FetchedOrder = context.Orders.Where(o => o.Id == order.Id).First();

                    // Si ya se completó salir
                    if (FetchedOrder.Status == OrderStates.Completado)
                    {
                        return;
                    }

                    // Simular 5 segundos de trabajo en 3 pasos

                    int delayLoop = 0;

                    logger.LogInformation("El elemento de trabajo en cola del order {order} se está iniciando.", order.Id);

                    try
                    {
                        while (!token.IsCancellationRequested && delayLoop < 3)
                        {
                            try
                            {
                                await Task.Delay(TimeSpan.FromSeconds(5), token);
                            }
                            catch (OperationCanceledException)
                            {
                                // Puede usar un mensaje diferente ya que son error diferentes
                                logger.LogInformation("El elemento de trabajo en cola del order {order} ha sido cancelado.", order.Id);
                                FetchedOrder.Status = OrderStates.Cancelado;
                                CreateLogDb(context, FetchedOrder, "Ha sido cancelado.");
                            }

                            ++delayLoop;

                            logger.LogInformation("El elemento de trabajo en cola del order {order} esta en ejecución. {DelayLoop}/3", order.Id, delayLoop);
                        }

                        if (delayLoop is 3)
                        {
                            logger.LogInformation("El elemento de trabajo en cola del order {order} esta completado.", order.Id);
                            FetchedOrder.Status = OrderStates.Completado;
                            CreateLogDb(context, FetchedOrder, "Ha sido completado.");
                        }
                        else
                        {
                            logger.LogInformation("El elemento de trabajo en cola del order {order} ha sido cancelado.", order.Id);
                            FetchedOrder.Status = OrderStates.Cancelado;
                            CreateLogDb(context, FetchedOrder, "Ha sido cancelado.");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Hubo un error ejecutando el order {order}. {error}", order.Id, ex.ToString());
                        CreateLogDb(context, FetchedOrder, ex.Message);
                    }

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Hubo un error ejecutando el order {order}. {error}", order.Id, ex.ToString());
            }
        }

        private static void CreateLogDb(AppSqlServerContext context, Order FetchedOrder, string error)
        {
            context.OrderLogs.Add(new OrderLog
            {
                Order = FetchedOrder,
                OrderId = FetchedOrder.Id,
                DateCreated = DateTime.Now,
                Error = error
            });
        }
    }
}
