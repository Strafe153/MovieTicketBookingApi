using Domain.Interfaces.Repositories;
using Quartz;

namespace MovieTicketBookingApi.Jobs.RecurringJobs;

public class MovieSessionFinishJob : IJob
{
    private readonly IMovieSessionsRepository _repository;

    public MovieSessionFinishJob(IMovieSessionsRepository repository)
    {
        _repository = repository;
    }

    public Task Execute(IJobExecutionContext context) => _repository.UpdateFinishedAsync();
}