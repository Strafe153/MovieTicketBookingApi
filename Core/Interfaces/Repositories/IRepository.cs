namespace Core.Interfaces.Repositories;

public interface IRepository<T>
{
    Task<IList<T>> GetAllAsync(int? pageNumber, int? pageSize);
    Task<T?> GetByIdAsync(Guid id);
    T Create(T entity);
    void Update(T entity);
    void Delete(T entity);
}
