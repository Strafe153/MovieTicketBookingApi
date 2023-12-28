using Core.Interfaces.BucketProviders;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MovieTicketBookingApi.HealthChecks;

public class CouchbaseHealthCheck : IHealthCheck
{
    private readonly IMovieTicketBookingBucketProvider _bucketProvider;

    public CouchbaseHealthCheck(IMovieTicketBookingBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var cluster = (await _bucketProvider.GetBucketAsync()).Cluster;
            await cluster.QueryAsync<object>("SELECT 1");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}