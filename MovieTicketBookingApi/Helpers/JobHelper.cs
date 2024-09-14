using Domain.Interfaces.Helpers;
using Quartz;

namespace MovieTicketBookingApi.Helpers;

public class JobHelper : IJobHelper
{
    private readonly ISchedulerFactory _schedulerFactory;

    public JobHelper(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task RunOneOffJob<T>(JobDataMap? dataMap) where T : IJob
    {
        var jobBuilder = JobBuilder.Create<T>();

        if (dataMap is not null)
        {
            jobBuilder = jobBuilder.UsingJobData(dataMap);
        }

        var job = jobBuilder.Build();

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .StartNow()
            .Build();

        var scheduler = await _schedulerFactory.GetScheduler();

        await scheduler.ScheduleJob(job, trigger);
    }
}
