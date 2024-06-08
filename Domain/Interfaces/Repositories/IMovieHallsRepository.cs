using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IMovieHallsRepository
{
	Task<IList<MovieHall>> GetAllAsync(int pageNumber, int pageSize);
	Task<MovieHall> GetByIdAsync(string id);
	Task InsertAsync(MovieHall movieHall);
	Task UpdateAsync(MovieHall movieHall);
	Task DeleteAsync(string id);
}
