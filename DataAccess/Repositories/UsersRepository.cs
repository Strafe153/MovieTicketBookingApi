using Core.Entities;
using Core.Interfaces.BucketProviders;
using Core.Interfaces.Repositories;

namespace DataAccess.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly IMovieTicketBookingBucketProvider _bucketProvider;

    public UsersRepository(IMovieTicketBookingBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task Create(User entity)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("users");

        await collection.InsertAsync(Guid.NewGuid().ToString(), entity);
    }

    public async Task Delete(Guid id)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("users");

        await collection.RemoveAsync(id.ToString());
    }

    public async Task<IList<User>> GetAllAsync(int? pageNumber, int? pageSize)
    {
        var pageIndex = pageNumber.HasValue ? pageNumber.Value : 0;
        var pageLimit = pageSize.HasValue ? pageSize.Value : 5;

        var cluster = (await _bucketProvider.GetBucketAsync()).Cluster;

        var usersQuery = await cluster.QueryAsync<User>($@"
            SELECT META(u.Id),
                   u.FirstName,
                   u.LastName,
                   u.Email,
                   u.BirthDate
            FROM users AS u
            OFFSET {pageIndex * pageLimit}
            TAKE {pageLimit}");

        return await usersQuery.Rows.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var cluster = (await _bucketProvider.GetBucketAsync()).Cluster;

        var userQuery = await cluster.QueryAsync<User>($@"
            SELECT META(u.Id),
                   u.FirstName,
                   u.LastName,
                   u.Email,
                   u.BirthDate
            FROM users AS u
            LIMIT 1");

        return await userQuery.Rows.FirstOrDefaultAsync();
    }

    public async Task Update(User entity)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("users");

        await collection.ReplaceAsync(entity.Id.ToString(), entity);
    }
}
