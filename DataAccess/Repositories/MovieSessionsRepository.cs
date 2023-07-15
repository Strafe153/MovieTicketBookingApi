using Core.Entities;
using Core.Interfaces.BucketProviders;
using Core.Interfaces.Repositories;

namespace DataAccess.Repositories;

public class MovieSessionsRepository : IMovieSessionsRepository
{
    private readonly IMovieTicketBookingBucketProvider _bucketProvider;

    public MovieSessionsRepository(IMovieTicketBookingBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task Create(MovieSession entity)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("movie-sessions");

        await collection.InsertAsync(Guid.NewGuid().ToString(), entity);
    }

    public async Task Delete(Guid id)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("movie-sessions");

        await collection.RemoveAsync(id.ToString());
    }

    public async Task<IList<MovieSession>> GetAllAsync(int? pageNumber, int? pageSize)
    {
        var pageIndex = pageNumber.HasValue ? pageNumber.Value : 0;
        var pageLimit = pageSize.HasValue ? pageSize.Value : 5;

        var cluster = (await _bucketProvider.GetBucketAsync()).Cluster;

        var movieSessionsQuery = await cluster.QueryAsync<MovieSession>($@"
            SELECT META(ms.Id),
                   ms.DateTime,
                   ms.MovieId,
                   ms.MovieHallId
            FROM movie-sessions
            OFFSET {pageIndex * pageLimit}
            TAKE {pageLimit}");

        return await movieSessionsQuery.Rows.ToListAsync();
    }

    public async Task<MovieSession?> GetByIdAsync(Guid id)
    {
        var cluster = (await _bucketProvider.GetBucketAsync()).Cluster;

        var movieSessionQuery = await cluster.QueryAsync<MovieSession>($@"
            SELECT META(ms.Id),
                   ms.DateTime,
                   ms.MovieId,
                   ms.MovieHallId
            FROM movie-sessions
            LIMIT 1");

        return await movieSessionQuery.Rows.FirstOrDefaultAsync();
    }

    public async Task Update(MovieSession entity)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("movie-sessions");

        await collection.ReplaceAsync(entity.Id.ToString(), entity);
    }
}
