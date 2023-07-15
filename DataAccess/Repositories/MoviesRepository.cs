using Core.Entities;
using Core.Interfaces.BucketProviders;
using Core.Interfaces.Repositories;

namespace DataAccess.Repositories;

public class MoviesRepository : IMoviesRepository
{
    private readonly IMovieTicketBookingBucketProvider _bucketProvider;

    public MoviesRepository(IMovieTicketBookingBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task Create(Movie entity)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("movies");

        await collection.InsertAsync(Guid.NewGuid().ToString(), entity);
    }

    public async Task Delete(Guid id)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("movies");

        await collection.RemoveAsync(id.ToString());
    }

    public async Task<IList<Movie>> GetAllAsync(int? pageNumber, int? pageSize)
    {
        var pageIndex = pageNumber.HasValue ? pageNumber.Value : 0;
        var pageLimit = pageSize.HasValue ? pageSize.Value : 5;

        var cluster = (await _bucketProvider.GetBucketAsync()).Cluster;

        var moviesQuery = await cluster.QueryAsync<Movie>($@"
            SELECT META(m.Id),
                   m.Title,
                   m.DurationInMinutes,
                   m.AgeRating
            FROM movies AS m
            OFFSET {pageIndex * pageLimit}
            TAKE {pageLimit}");

        return await moviesQuery.Rows.ToListAsync();
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        var cluster = (await _bucketProvider.GetBucketAsync()).Cluster;

        var movieQuery = await cluster.QueryAsync<Movie>($@"
            SELECT META(m.Id),
                   m.Title,
                   m.DurationInMinutes,
                   m.AgeRating
            FROM movies AS m
            LIMIT 1");

        return await movieQuery.Rows.FirstOrDefaultAsync();
    }

    public async Task Update(Movie entity)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("movies");

        await collection.ReplaceAsync(entity.Id.ToString(), entity);
    }
}
