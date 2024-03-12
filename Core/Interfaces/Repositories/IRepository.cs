namespace Core.Interfaces.Repositories;

public interface IRepository<T>
{
	Task<IList<T>> GetAllAsync(int pageNumber, int pageSize);
	Task<T> GetByIdAsync(string id);
	Task InsertAsync(T entity);
	Task UpdateAsync(T entity);
	Task DeleteAsync(string id);
}

