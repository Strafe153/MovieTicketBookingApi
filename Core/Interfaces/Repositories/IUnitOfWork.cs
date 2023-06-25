namespace Core.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
