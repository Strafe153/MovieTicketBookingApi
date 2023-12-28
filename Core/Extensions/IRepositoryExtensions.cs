using Core.Interfaces.Repositories;
using Couchbase.Core.Exceptions.KeyValue;

namespace Core.Extensions;

public static class IRepositoryExtensions
{
    public static async Task<T> GetByIdOrThrowAsync<T>(this IRepository<T> repository, string id)
    {
        try
        {
            return await repository.GetByIdAsync(id);
        }
        catch (DocumentNotFoundException)
        {
            throw new NullReferenceException($"{typeof(T).Name} with id '{id}' does not exist.");
        }
    }
}
