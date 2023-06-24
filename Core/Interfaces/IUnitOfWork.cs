namespace Core.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
