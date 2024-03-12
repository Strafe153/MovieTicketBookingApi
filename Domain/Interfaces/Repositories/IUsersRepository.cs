using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IUsersRepository : IRepository<User>
{
	Task<User?> GetByEmailAsync(string email);
}
