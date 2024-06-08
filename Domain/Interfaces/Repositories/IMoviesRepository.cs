using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IMoviesRepository
{
	Task<IList<Movie>> GetAllAsync(int pageNumber, int pageSize);
	Task<Movie> GetByIdAsync(string id);
	Task InsertAsync(Movie movie);
	Task UpdateAsync(Movie movie);
	Task DeleteAsync(string id);
}
