using Core.Interfaces.Jobs;
using Core.Interfaces.Repositories;

namespace MovieTicketBookingApi.Jobs.RecurringJobs;

public class MovieSessionFinishJob : IAsyncJob
{
	private readonly IMovieSessionsRepository _repository;

	public MovieSessionFinishJob(IMovieSessionsRepository repository)
	{
		_repository = repository;
	}

	public async Task ExecuteAsync(params object[] parameters) => await _repository.UpdateFinishedAsync();
}

