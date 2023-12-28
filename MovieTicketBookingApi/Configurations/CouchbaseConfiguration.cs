using Core.Interfaces.BucketProviders;
using Core.Shared.Constants;
using Core.Shared.ContractResolvers;
using Couchbase.Core.IO.Serializers;
using Couchbase.Extensions.DependencyInjection;
using DataAccess;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MovieTicketBookingApi.Configurations;

public static class CouchbaseConfiguration
{
    public static void ConfigureCouchbase(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddCouchbase(options =>
            {
                configuration.GetSection(CouchbaseConstants.CouchbaseOptionsName).Bind(options);

                options.Serializer = new DefaultSerializer(
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        DateParseHandling = DateParseHandling.DateTimeOffset
                    },
                    new JsonSerializerSettings
                    {
                        ContractResolver = new IgnoreIdContractResolver()
                    });
            })
            .AddCouchbaseBucket<IMovieTicketBookingBucketProvider>(CouchbaseConstants.BucketName)
            .AddSingleton<DatabaseSetupper>();
    }

    public static async Task SetupDatabase(this WebApplication application)
    {
        var dbSetupper = application.Services.GetRequiredService<DatabaseSetupper>();
        await dbSetupper.SetupDatabase();
    }

    public static void ConfigureCouchbaseLifetime(this WebApplication application)
    {
        application.Lifetime.ApplicationStopped.Register(async () =>
        {
            await application.Services.GetRequiredService<ICouchbaseLifetimeService>().CloseAsync();
        });
    }
}