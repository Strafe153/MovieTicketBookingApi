using Domain.Interfaces.Jobs;
using Domain.Interfaces.Repositories;

namespace MovieTicketBookingApi.Jobs.RecurringJobs;

public class MovieSessionFinishJob : IAsyncJob
{
	private readonly IMovieSessionsRepository _repository;

	public MovieSessionFinishJob(IMovieSessionsRepository repository)
	{
		_repository = repository;
	}

	public Task ExecuteAsync() => _repository.UpdateFinishedAsync();
}

