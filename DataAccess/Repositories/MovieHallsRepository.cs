using System.Text.Json;
using Couchbase.KeyValue;
using Couchbase.Query;
using Domain.Entities;
using Domain.Extensions;
using Domain.Interfaces.BucketProviders;
using Domain.Interfaces.Repositories;
using Domain.Shared.Constants;

namespace DataAccess.Repositories;

public class MovieHallsRepository(IMovieTicketBookingBucketProvider bucketProvider)
    : Repository(bucketProvider), IMovieHallsRepository
{
    public async Task DeleteAsync(string id)
    {
        var isActive = JsonNamingPolicy.CamelCase.ConvertName(nameof(MovieHall.IsActive));

        var collection = await GetCollectionAsync();

        await collection.MutateInAsync(id, specs => specs.Upsert(isActive, false));
    }

    public async Task<IList<MovieHall>> GetAllAsync(int pageNumber, int pageSize)
    {
        var offset = GetOffset(pageNumber, pageSize);

        var queryOptions = new QueryOptions()
            .Parameter("offset", offset)
            .Parameter("pageSize", pageSize);

        var query = $@"
            SELECT META(mh).id,
                   mh.name,
                   mh.numberOfSeats,
                   ARRAY_AGG(ms) AS MovieSessions
            FROM `{CouchbaseConstants.MovieHallsCollection}` AS mh
            LEFT JOIN `{CouchbaseConstants.MovieSessionsCollection}` AS ms ON ms.movieHallId = META(mh).id
            GROUP BY mh
            ORDER BY mh.name, mh.numberOfSeats
            OFFSET $offset
            LIMIT $pageSize";

        var scope = await GetScopeAsync();
        var queryResult = await scope.QueryAsync<MovieHall>(query, queryOptions);
        var movieHalls = await queryResult.Rows.ToListAsync();

        return movieHalls;
    }

    public async Task<MovieHall> GetByIdAsync(string id)
    {
        var queryOptions = new QueryOptions().Parameter("id", id);

        var query = $@"
            SELECT META(mh).id,
                   mh.name,
                   mh.numberOfSeats,
                   ARRAY_AGG(ms) AS MovieSessions
            FROM `{CouchbaseConstants.MovieHallsCollection}` AS mh
            LEFT JOIN `{CouchbaseConstants.MovieSessionsCollection}` AS ms ON ms.movieHallId = META(mh).id
            GROUP BY mh
            WHERE META(mh).id = $id";

        var scope = await GetScopeAsync();
        var queryResult = await scope.QueryAsync<MovieHall>(query, queryOptions);
        var movieHall = await queryResult.FirstOrDefaultAsync();

        return movieHall;
    }

    public async Task InsertAsync(MovieHall movieHall)
    {
        var collection = await GetCollectionAsync();
        await collection.InsertAsync(movieHall.Id.ToString(), movieHall);
    }

    public async Task UpdateAsync(MovieHall movieHall)
    {
        var collection = await GetCollectionAsync();
        await collection.ReplaceAsync(movieHall.Id.ToString(), movieHall);
    }

    protected override Task<ICouchbaseCollection> GetCollectionAsync() =>
        _bucketProvider.GetCollectionAsync(CouchbaseConstants.MovieHallsCollection);
}
