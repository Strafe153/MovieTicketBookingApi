using Core.Shared.Constants;
using Hangfire;
using MovieTicketBookingApi.Jobs.RecurringJobs;

namespace MovieTicketBookingApi.Jobs;

public static class RecurringJobsRegistry
{
	public static void RegisterRecurringJobs()
	{
		RecurringJob.AddOrUpdate<MovieSessionFinishJob>(nameof(MovieSessionFinishJob), j => j.ExecuteAsync(), CronConstants.Quarterly);
		RecurringJob.AddOrUpdate<TicketsFinishJob>(nameof(TicketsFinishJob), j => j.ExecuteAsync(), CronConstants.Quarterly);
	}
}