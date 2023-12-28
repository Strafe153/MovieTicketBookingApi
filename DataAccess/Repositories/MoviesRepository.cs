using Core.Entities;
using Core.Extensions;
using Core.Interfaces.BucketProviders;
using Core.Interfaces.Repositories;
using Core.Shared.Constants;

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
        await collection.RemoveAsync(id);
    }

    public async Task<IList<Movie>> GetAllAsync(int pageNumber, int pageSize)
    {
        var scope = await _bucketProvider.GetScopeAsync();
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
            OFFSET {(pageNumber - 1) * pageSize}
            LIMIT {pageSize}";

        var queryResult = await scope.QueryAsync<Movie>(query);

        return await queryResult.Rows.ToListAsync();
    }

    public async Task<Movie> GetByIdAsync(string id)
    {
        var scope = await _bucketProvider.GetScopeAsync();
        var query = $@"
            SELECT META(m).id,
                   m.title,
                   m.durationInMinutes,
                   m.ageRating,
                   ARRAY_AGG(ms) AS MovieSessions
            FROM `{CouchbaseConstants.MoviesCollection}` AS m
            LEFT JOIN `{CouchbaseConstants.MovieSessionsCollection}` AS ms ON ms.movieId = META(m).id
            WHERE META(m).id = '{id}'
            GROUP BY m";

        var queryResult = await scope.QueryAsync<Movie>(query);

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
