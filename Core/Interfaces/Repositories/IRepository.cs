namespace Core.Interfaces.Repositories;

public interface IRepository<T>
{
    Task<IList<T>> GetAllAsync(int? pageNumber, int? pageSize);
    Task<T?> GetByIdAsync(Guid id);
    Task Create(T entity);
    Task Update(T entity);
    Task Delete(Guid id);
}
