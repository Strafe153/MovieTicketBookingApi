using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IUsersRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
