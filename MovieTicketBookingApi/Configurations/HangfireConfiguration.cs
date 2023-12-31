using Hangfire;
using HangfireBasicAuthenticationFilter;
using MovieTicketBookingApi.Configurations.ConfigurationModels;

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

    public static void UseAuthenticatedHangfireDashboard(this WebApplication application, IConfiguration configuration)
    {
        var hangfireConfiguration = configuration
            .GetSection(HangfireOptions.HangfireSectionName)
            .Get<HangfireOptions>()!;

        application.UseHangfireDashboard(options: new DashboardOptions
        {
            DashboardTitle = typeof(Program).Assembly.GetName().Name,
            Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter
                {
                    User = hangfireConfiguration.User,
                    Pass = hangfireConfiguration.Password
                }
            }
        });
    }
}
