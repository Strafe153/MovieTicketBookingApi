using MovieTicketBookingApi.Jobs.RecurringJobs;
using Quartz;
using Quartz.AspNetCore;

namespace MovieTicketBookingApi.Configurations;

public static class QuartzConfiguration
{
    public static void ConfigureQuartz(this IServiceCollection services)
    {
        services.AddQuartz(c => c
            .AddMovieSessionFinishJob()
            .AddTicketsFinishJob());

        services.AddQuartzServer(options => options.WaitForJobsToComplete = true);
    }

    private static IServiceCollectionQuartzConfigurator AddMovieSessionFinishJob(
        this IServiceCollectionQuartzConfigurator configurator)
    {
        configurator.ScheduleJob<MovieSessionFinishJob>(trigger => trigger
            .WithIdentity(nameof(MovieSessionFinishJob))
            .WithSimpleSchedule(b => b
                .WithIntervalInMinutes(15)
                .RepeatForever()));

        return configurator;
    }

    private static IServiceCollectionQuartzConfigurator AddTicketsFinishJob(
        this IServiceCollectionQuartzConfigurator configurator)
    {
        configurator.ScheduleJob<TicketsFinishJob>(trigger => trigger
            .WithIdentity(nameof(TicketsFinishJob))
            .WithSimpleSchedule(b => b
                .WithIntervalInMinutes(15)
                .RepeatForever()));

        return configurator;
    }
}
