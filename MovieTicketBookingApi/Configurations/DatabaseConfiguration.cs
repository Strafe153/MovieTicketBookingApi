using Core.Interfaces.BucketProviders;
using Couchbase.Extensions.DependencyInjection;
using MovieTicketBookingApi.Configurations.ConfigurationModels;

namespace MovieTicketBookingApi.Configurations;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddCouchbase(options =>
            {
                var couchbaseOptions = new CouchbaseOptions();
                configuration.GetSection("CouchbaseSettings").Bind(couchbaseOptions);

                options.ConnectionString = couchbaseOptions.ConnectionString;
                options.UserName = couchbaseOptions.UserName;
                options.Password = couchbaseOptions.Password;
                options.HttpIgnoreRemoteCertificateMismatch = true;
                options.KvIgnoreRemoteCertificateNameMismatch = true;
            })
            .AddCouchbaseBucket<IMovieTicketBookingBucketProvider>("movie-ticket-booking");
    }
}
