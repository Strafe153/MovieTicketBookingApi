using System.Text.Json;
using Couchbase.KeyValue;
using Couchbase.Query;
using Domain.Entities;
using Domain.Extensions;
using Domain.Interfaces.BucketProviders;
using Domain.Interfaces.Repositories;
using Domain.Shared.Constants;

namespace DataAccess.Repositories;

public class MoviesRepository(IMovieTicketBookingBucketProvider bucketProvider)
    : Repository(bucketProvider), IMoviesRepository
{
    public async Task DeleteAsync(string id)
    {
        var isActive = JsonNamingPolicy.CamelCase.ConvertName(nameof(Movie.IsActive));

        var collection = await GetCollectionAsync();

        await collection.MutateInAsync(id, specs => specs.Upsert(isActive, false));
    }

    public async Task<IList<Movie>> GetAllAsync(int pageNumber, int pageSize)
    {
        var offset = GetOffset(pageNumber, pageSize);

        var queryOptions = new QueryOptions()
            .Parameter("offset", offset)
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

        var scope = await GetScopeAsync();
        var queryResult = await scope.QueryAsync<Movie>(query, queryOptions);
        var movies = await queryResult.Rows.ToListAsync();

        return movies;
    }

    public async Task<Movie> GetByIdAsync(string id)
    {
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

        var scope = await GetScopeAsync();
        var queryResult = await scope.QueryAsync<Movie>(query, queryOptions);
        var movie = await queryResult.FirstOrDefaultAsync();

        return movie;
    }

    public async Task InsertAsync(Movie movie)
    {
        var collection = await GetCollectionAsync();
        await collection.InsertAsync(movie.Id.ToString(), movie);
    }

    public async Task UpdateAsync(Movie movie)
    {
        var collection = await GetCollectionAsync();
        await collection.ReplaceAsync(movie.Id.ToString(), movie);
    }

    protected override Task<ICouchbaseCollection> GetCollectionAsync() =>
        _bucketProvider.GetCollectionAsync(CouchbaseConstants.MoviesCollection);
}
