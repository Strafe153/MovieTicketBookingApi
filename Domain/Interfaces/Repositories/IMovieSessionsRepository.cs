using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IMovieSessionsRepository
{
	Task<IList<MovieSession>> GetAllAsync(int pageNumber, int pageSize);
	Task<MovieSession> GetByIdAsync(string id);
	Task InsertAsync(MovieSession movieSession);
	Task UpdateAsync(MovieSession movieSession);
	Task UpdateFinishedAsync();
}
