using Hangfire;

namespace MovieTicketBookingApi.Configurations;

public static class HangfireConfiguration
{
    public static void ConfigureHangfire(this IServiceCollection services)
    {
        services.AddHangfire(options =>
        {
            options
                .UseInMemoryStorage()
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings();
        });

        services.AddHangfireServer();
    }
}
