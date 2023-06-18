using Core.Entities;

namespace Core.Interfaces;

public interface IMoviesRepository
{
    Task<IList<Movie>> GetAll();
    Task<Movie?> GetById(Guid id);
}
