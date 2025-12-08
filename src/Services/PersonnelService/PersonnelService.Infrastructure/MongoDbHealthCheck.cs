using Microsoft.Extensions.Diagnostics.HealthChecks;
using PersonnelService.Infrastructure.Context;

namespace PersonnelService.Infrastructure
{
    public class MongoDbHealthCheck : IHealthCheck
    {
        private readonly MongoDbContext _dbContext;

        public MongoDbHealthCheck(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _dbContext.Personnel.EstimatedDocumentCountAsync(
                    new MongoDB.Driver.EstimatedDocumentCountOptions(),
                    cancellationToken);

                return HealthCheckResult.Healthy("MongoDB is reachable.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("MongoDB is not reachable.", ex);
            }
        }
    }
}