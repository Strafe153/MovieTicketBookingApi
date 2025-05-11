using System.Text.Json;
using Couchbase.KeyValue;
using Couchbase.Query;
using Domain.Entities;
using Domain.Extensions;
using Domain.Interfaces.BucketProviders;
using Domain.Interfaces.Repositories;
using Domain.Shared.Constants;

namespace DataAccess.Repositories;

public class UsersRepository(IMovieTicketBookingBucketProvider bucketProvider)
	: Repository(bucketProvider), IUsersRepository
{
    public async Task DeleteAsync(string id)
	{
        var isActive = JsonNamingPolicy.CamelCase.ConvertName(nameof(User.IsActive));

        var collection = await GetCollectionAsync();

		await collection.MutateInAsync(id, specs => specs.Upsert(isActive, false));
	}

	public async Task<IList<User>> GetAllAsync(int pageNumber, int pageSize)
	{
		var offset = GetOffset(pageNumber, pageSize);

		var queryOptions = new QueryOptions()
			.Parameter("offset", offset)
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

        var scope = await GetDefaultScopeAsync();
        var queryResult = await scope.QueryAsync<User>(query, queryOptions);
		var users = await queryResult.Rows.ToListAsync();

		return users;
	}

	public async Task<User?> GetByEmailAsync(string email)
	{
		var queryOptions = new QueryOptions().Parameter("email", email);

		var query = $@"
            SELECT META(u).id,
                   u.email,
                   u.passwordHash,
                   u.passwordSalt
            FROM `{CouchbaseConstants.UsersCollection}` AS u
            WHERE u.email = $email";

        var scope = await GetScopeAsync();
        var queryResult = await scope.QueryAsync<User>(query, queryOptions);
		var user = await queryResult.FirstOrDefaultAsync();

		return user;
	}

	public async Task<User> GetByIdAsync(string id)
	{
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

        var scope = await GetDefaultScopeAsync();
        var queryResult = await scope.QueryAsync<User>(query, queryOptions);
		var user = await queryResult.FirstOrDefaultAsync();

		return user;
	}

	public async Task InsertAsync(User user)
	{
		var collection = await GetCollectionAsync();
		await collection.InsertAsync(user.Id.ToString(), user);
	}

	public async Task UpdateAsync(User user)
	{
		var collection = await GetCollectionAsync();
		await collection.ReplaceAsync(user.Id.ToString(), user);
    }

    protected override Task<ICouchbaseCollection> GetCollectionAsync() =>
        _bucketProvider.GetCollectionAsync(CouchbaseConstants.UsersCollection);
}
