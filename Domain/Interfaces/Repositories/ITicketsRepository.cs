using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface ITicketsRepository : IRepository<Ticket>
{
	Task<IList<Ticket>> GetByUserIdAsync(int pageNumber, int pageSize, string userId);
	Task UpdateFinishedAsync();
}
