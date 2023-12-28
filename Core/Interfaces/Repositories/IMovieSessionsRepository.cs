using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IMovieSessionsRepository : IRepository<MovieSession>
{
    Task DeleteFinished();
}
