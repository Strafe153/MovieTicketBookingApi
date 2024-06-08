using Domain.Interfaces.Jobs;
using Domain.Shared.Constants;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using MovieTicketBookingApi.Configurations.ConfigurationModels;
using MovieTicketBookingApi.Jobs.FireAndForgetJobs;
using MovieTicketBookingApi.Jobs.RecurringJobs;

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
		var hangfireOptions = configuration
			.GetSection(HangfireOptions.SectionName)
			.Get<HangfireOptions>()!;

		application.UseHangfireDashboard(options: new DashboardOptions
		{
			DashboardTitle = typeof(Program).Assembly.GetName().Name,
			Authorization = new[]
			{
				new HangfireCustomBasicAuthenticationFilter
				{
					User = hangfireOptions.User,
					Pass = hangfireOptions.Password
				}
			}
		});
	}

	public static void RegisterJobs(this IServiceCollection services) =>
        services.AddScoped<IRegistrationEmailJob, RegistrationEmailJob>();

    public static void RegisterRecurringJobs()
	{
        RecurringJob.AddOrUpdate<MovieSessionFinishJob>(nameof(MovieSessionFinishJob), j => j.ExecuteAsync(), CronConstants.Quarterly);
        RecurringJob.AddOrUpdate<TicketsFinishJob>(nameof(TicketsFinishJob), j => j.ExecuteAsync(), CronConstants.Quarterly);
    }
}
