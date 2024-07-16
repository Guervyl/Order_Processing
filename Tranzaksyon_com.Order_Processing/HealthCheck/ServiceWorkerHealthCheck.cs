using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Tranzaksyon.Database.AppDbContext;

namespace Tranzaksyon_com.Order_Processing.HealthCheck
{
    public sealed class ServiceWorkerHealthCheck(AppSqlServerContext dbContext) : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var lastLog = dbContext.OrderLogs.OrderBy(o=>o.DateCreated).LastOrDefault();

                // Tomar en cuenta solo los de 30 minutos
                if (lastLog?.DateCreated.AddMinutes(30) >= DateTime.Now)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy(lastLog.Error));
                }

                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(ex.ToString()));
            }
        }
    }
}
