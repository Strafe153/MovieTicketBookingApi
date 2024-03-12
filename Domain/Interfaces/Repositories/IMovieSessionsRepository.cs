using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IMovieSessionsRepository : IRepository<MovieSession>
{
	Task UpdateFinishedAsync();
}
