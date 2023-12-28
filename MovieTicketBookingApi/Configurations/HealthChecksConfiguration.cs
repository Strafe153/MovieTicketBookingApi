using MovieTicketBookingApi.HealthChecks;

namespace MovieTicketBookingApi.Configurations;

public static class HealthChecksConfiguration
{
    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        services
            .AddGrpcHealthChecks()
            .AddCheck<CouchbaseHealthCheck>("Couchbase");
    }
}
