using Core.Entities;
using Core.Extensions;
using Core.Interfaces.BucketProviders;
using Core.Interfaces.Repositories;
using Core.Shared.Constants;
using Couchbase.Query;

namespace DataAccess.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly IMovieTicketBookingBucketProvider _bucketProvider;

    public UsersRepository(IMovieTicketBookingBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task DeleteAsync(string id)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.UsersCollection);
        await collection.RemoveAsync(id);
    }

    public async Task<IList<User>> GetAllAsync(int pageNumber, int pageSize)
    {
        var scope = await (await _bucketProvider.GetBucketAsync()).ScopeAsync(CouchbaseConstants.DefaultScope);

        var queryOptions = new QueryOptions()
            .Parameter("offset", (pageNumber - 1) * pageSize)
            .Parameter("pageSize", pageSize);

        var query = $@"
            SELECT META(u).id,
                   u.firstName,
                   u.lastName,
                   u.email,
                   u.birthDate,
                   ARRAY_AGG(t) AS Tickets
            FROM `{CouchbaseConstants.UsersCollection}` AS u
            LEFT JOIN `{CouchbaseConstants.TicketsCollection}` AS t ON t.userId = META(u).id
            GROUP BY u
            ORDER BY u.email, u.firstName, u.lastName, u.birthDate
            OFFSET $offset
            LIMIT $pageSize";

        var queryResult = await scope.QueryAsync<User>(query, queryOptions);

        return await queryResult.Rows.ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var scope = await _bucketProvider.GetScopeAsync();
        var queryOptions = new QueryOptions().Parameter("email", email);

        var query = $@"
            SELECT META(u).id,
                   u.email,
                   u.passwordHash,
                   u.passwordSalt
            FROM `{CouchbaseConstants.UsersCollection}` AS u
            WHERE u.email = $email";

        var queryResult = await scope.QueryAsync<User>(query, queryOptions);
        return await queryResult.FirstOrDefaultAsync();
    }

    public async Task<User> GetByIdAsync(string id)
    {
        var scope = await (await _bucketProvider.GetBucketAsync()).ScopeAsync(CouchbaseConstants.DefaultScope);
        var queryOptions = new QueryOptions().Parameter("id", id);

        var query = $@"
            SELECT META(u).id,
                   u.firstName,
                   u.lastName,
                   u.email,
                   u.birthDate,
                   ARRAY_AGG(t) AS Tickets
            FROM `{CouchbaseConstants.UsersCollection}` AS u
            LEFT JOIN `{CouchbaseConstants.TicketsCollection}` AS t ON t.userId = META(u).id
            WHERE META(u).id = $id
            GROUP BY u";

        var queryResult = await scope.QueryAsync<User>(query, queryOptions);

        return await queryResult.FirstOrDefaultAsync();
    }

    public async Task InsertAsync(User entity)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.UsersCollection);
        await collection.InsertAsync(entity.Id.ToString(), entity);
    }

    public async Task UpdateAsync(User entity)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.UsersCollection);
        await collection.ReplaceAsync(entity.Id.ToString(), entity);
    }
}
