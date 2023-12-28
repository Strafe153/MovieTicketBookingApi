using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface ITicketsRepository : IRepository<Ticket>
{
    Task<IList<Ticket>> GetByUserIdAsync(int pageNumber, int pageSize, string userId);
}
