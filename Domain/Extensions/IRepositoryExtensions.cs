using Domain.Interfaces.Repositories;

namespace Domain.Extensions;

public static class IRepositoryExtensions
{
	public static async Task<T> GetByIdOrThrowAsync<T>(this IRepository<T> repository, string id)
	{
		var entity = await repository.GetByIdAsync(id)
			?? throw new NullReferenceException($"{typeof(T).Name} with id '{id}' does not exist.");
		
		return entity;
	}
}
