using Core.Entities;
using Core.Extensions;
using Core.Interfaces.BucketProviders;
using Core.Interfaces.Repositories;
using Core.Shared.Constants;

namespace DataAccess.Repositories;

public class MovieHallsRepository : IMovieHallsRepository
{
    private readonly IMovieTicketBookingBucketProvider _bucketProvider;

    public MovieHallsRepository(IMovieTicketBookingBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task DeleteAsync(string id)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.MovieHallsCollection);
        await collection.RemoveAsync(id);
    }

    public async Task<IList<MovieHall>> GetAllAsync(int pageNumber, int pageSize)
    {
        var scope = await _bucketProvider.GetScopeAsync();
        var query = $@"
            SELECT META(mh).id,
                   mh.name,
                   mh.numberOfSeats,
                   ARRAY_AGG(ms) AS MovieSessions
            FROM `{CouchbaseConstants.MovieHallsCollection}` AS mh
            LEFT JOIN `{CouchbaseConstants.MovieSessionsCollection}` AS ms ON ms.movieHallId = META(mh).id
            GROUP BY mh
            ORDER BY mh.name, mh.numberOfSeats
            OFFSET {(pageNumber - 1) * pageSize}
            LIMIT {pageSize}";

        var queryResult = await scope.QueryAsync<MovieHall>(query);

        return await queryResult.Rows.ToListAsync();
    }

    public async Task<MovieHall> GetByIdAsync(string id)
    {
        var scope = await _bucketProvider.GetScopeAsync();
        var query = $@"
            SELECT META(mh).id,
                   mh.name,
                   mh.numberOfSeats,
                   ARRAY_AGG(ms) AS MovieSessions
            FROM `{CouchbaseConstants.MovieHallsCollection}` AS mh
            LEFT JOIN `{CouchbaseConstants.MovieSessionsCollection}` AS ms ON ms.movieHallId = META(mh).id
            GROUP BY mh
            WHERE META(mh).id = 'id'";

        var queryResult = await scope.QueryAsync<MovieHall>(query);

        return await queryResult.FirstOrDefaultAsync();
    }

    public async Task InsertAsync(MovieHall entity)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.MovieHallsCollection);
        await collection.InsertAsync(entity.Id.ToString(), entity);
    }

    public async Task UpdateAsync(MovieHall entity)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.MovieHallsCollection);
        await collection.ReplaceAsync(entity.Id.ToString(), entity);
    }
}
