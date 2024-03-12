using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface ITicketsRepository
{
	Task<IList<Ticket>> GetAllAsync(int pageNumber, int pageSize);
	Task<IList<Ticket>> GetByUserIdAsync(int pageNumber, int pageSize, string userId);
	Task<Ticket> GetByIdAsync(string id);
	Task InsertAsync(Ticket ticket);
	Task UpdateFinishedAsync();
}
