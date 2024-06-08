using Domain.Interfaces.Jobs;
using Domain.Interfaces.Repositories;

namespace MovieTicketBookingApi.Jobs.RecurringJobs;

public class TicketsFinishJob : IAsyncJob
{
	private readonly ITicketsRepository _repository;

	public TicketsFinishJob(ITicketsRepository repository)
	{
		_repository = repository;
	}

	public Task ExecuteAsync() => _repository.UpdateFinishedAsync();
}
