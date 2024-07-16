using Microsoft.Extensions.Diagnostics.HealthChecks;
using Tranzaksyon.Database.AppDbContext;

namespace Tranzaksyon_com.Order_Processing.HealthCheck
{
    public sealed class DatabaseHealthCheck(AppSqlServerContext dbContext) : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                dbContext.Orders.First();
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(ex.ToString()));
            }
        }
    }
}
