using Core.Interfaces.Jobs;
using Core.Interfaces.Repositories;

namespace MovieTicketBookingApi.Jobs.RecurringJobs;

public class TicketsFinishJob : IAsyncJob
{
    private readonly ITicketsRepository _repository;

    public TicketsFinishJob(ITicketsRepository repository)
    {
        _repository = repository;   
    }

    public async Task ExecuteAsync(params object[] parameters) => await _repository.UpdateFinishedAsync();
}
