using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IUsersRepository
{
	Task<IList<User>> GetAllAsync(int pageNumber, int pageSize);
	Task<User> GetByIdAsync(string id);
	Task<User?> GetByEmailAsync(string email);
	Task InsertAsync(User user);
	Task UpdateAsync(User user);
	Task DeleteAsync(string id);
}
