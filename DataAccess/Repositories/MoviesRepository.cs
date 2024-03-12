using Core.Entities;
using Core.Extensions;
using Core.Interfaces.BucketProviders;
using Core.Interfaces.Repositories;
using Core.Shared.Constants;
using Couchbase.KeyValue;
using Couchbase.Query;
using System.Text.Json;

namespace DataAccess.Repositories;

public class MoviesRepository : IMoviesRepository
{
    private readonly IMovieTicketBookingBucketProvider _bucketProvider;

    public MoviesRepository(IMovieTicketBookingBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task DeleteAsync(string id)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.MoviesCollection);

        await collection.MutateInAsync(id, specs =>
            specs.Upsert(JsonNamingPolicy.CamelCase.ConvertName(nameof(Movie.IsActive)), false));
    }

    public async Task<IList<Movie>> GetAllAsync(int pageNumber, int pageSize)
    {
        var scope = await _bucketProvider.GetScopeAsync();

        var queryOptions = new QueryOptions()
            .Parameter("offset", (pageNumber - 1) * pageSize)
            .Parameter("pageSize", pageSize);

        var query = $@"
            SELECT META(m).id,
                   m.title,
                   m.durationInMinutes,
                   m.ageRating,
                   ARRAY_AGG(ms) AS MovieSessions
            FROM `{CouchbaseConstants.MoviesCollection}` AS m
            LEFT JOIN `{CouchbaseConstants.MovieSessionsCollection}` AS ms ON ms.movieId = META(m).id
            GROUP BY m
            ORDER BY m.title, m.durationInMinutes, m.ageRating
            OFFSET $offset
            LIMIT $pageSize";

        var queryResult = await scope.QueryAsync<Movie>(query, queryOptions);

        return await queryResult.Rows.ToListAsync();
    }

    public async Task<Movie> GetByIdAsync(string id)
    {
        var scope = await _bucketProvider.GetScopeAsync();
        var queryOptions = new QueryOptions().Parameter("id", id);

        var query = $@"
            SELECT META(m).id,
                   m.title,
                   m.durationInMinutes,
                   m.ageRating,
                   ARRAY_AGG(ms) AS MovieSessions
            FROM `{CouchbaseConstants.MoviesCollection}` AS m
            LEFT JOIN `{CouchbaseConstants.MovieSessionsCollection}` AS ms ON ms.movieId = META(m).id
            WHERE META(m).id = $id
            GROUP BY m";

        var queryResult = await scope.QueryAsync<Movie>(query, queryOptions);

        return await queryResult.FirstOrDefaultAsync();
    }

    public async Task InsertAsync(Movie entity)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.MoviesCollection);
        await collection.InsertAsync(entity.Id.ToString(), entity);
    }

    public async Task UpdateAsync(Movie entity)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.MoviesCollection);
        await collection.ReplaceAsync(entity.Id.ToString(), entity);
    }
}
