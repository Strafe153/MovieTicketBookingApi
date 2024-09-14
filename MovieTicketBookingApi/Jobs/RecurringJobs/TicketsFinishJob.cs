using Domain.Interfaces.Repositories;
using Quartz;

namespace MovieTicketBookingApi.Jobs.RecurringJobs;

public class TicketsFinishJob : IJob
{
    private readonly ITicketsRepository _repository;

    public TicketsFinishJob(ITicketsRepository repository)
    {
        _repository = repository;
    }

    public Task Execute(IJobExecutionContext context) => _repository.UpdateFinishedAsync();
}
