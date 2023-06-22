namespace Core.Interfaces;

public interface IRepository<T>
{
    Task<IList<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    T Create(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}
