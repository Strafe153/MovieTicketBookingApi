using Core.Entities;
using Core.Interfaces.BucketProviders;
using Core.Interfaces.Repositories;

namespace DataAccess.Repositories;

public class MovieHallsRepository : IMovieHallsRepository
{
    private readonly IMovieTicketBookingBucketProvider _bucketProvider;

    public MovieHallsRepository(IMovieTicketBookingBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task Create(MovieHall entity)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("movie-halls");

        await collection.InsertAsync(Guid.NewGuid().ToString(), entity);
    }

    public async Task Delete(Guid id)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("movie-halls");

        await collection.RemoveAsync(id.ToString());
    }

    public async Task<IList<MovieHall>> GetAllAsync(int? pageNumber, int? pageSize)
    {
        var pageIndex = pageNumber.HasValue ? pageNumber.Value : 0;
        var pageLimit = pageSize.HasValue ? pageSize.Value : 5;

        var cluster = (await _bucketProvider.GetBucketAsync()).Cluster;
        var movieHallsQuery = await cluster.QueryAsync<MovieHall>($@"
            SELECT META(mh.Id),
                   mh.Name,
                   mh.NumberOfSeats
            FROM movie-halls AS mh
            OFFSET {pageIndex * pageLimit}
            TAKE {pageLimit}");

        return await movieHallsQuery.Rows.ToListAsync();
    }

    public async Task<MovieHall?> GetByIdAsync(Guid id)
    {
        var cluster = (await _bucketProvider.GetBucketAsync()).Cluster;
        var movieHallQuery = await cluster.QueryAsync<MovieHall>($@"
            SELECT META(mh.Id),
                   mh.Name,
                   mh.NumberOfSeats
            FROM movie-halls AS mh
            LIMIT 1");

        return await movieHallQuery.Rows.FirstOrDefaultAsync();
    }

    public async Task Update(MovieHall entity)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("movie-halls");

        await collection.ReplaceAsync(entity.Id.ToString(), entity);
    }
}
