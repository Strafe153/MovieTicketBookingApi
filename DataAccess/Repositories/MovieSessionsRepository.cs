using Core.Entities;
using Core.Extensions;
using Core.Interfaces.BucketProviders;
using Core.Interfaces.Repositories;
using Core.Shared.Constants;
using Couchbase.Query;

namespace DataAccess.Repositories;

public class MovieSessionsRepository : IMovieSessionsRepository
{
    private readonly IMovieTicketBookingBucketProvider _bucketProvider;

    public MovieSessionsRepository(IMovieTicketBookingBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task DeleteAsync(string id)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.MovieSessionsCollection);
        await collection.RemoveAsync(id);
    }

    public async Task UpdateFinishedAsync()
    {
        var scope = await _bucketProvider.GetScopeAsync();

        var query = $@"
            UPDATE `{CouchbaseConstants.MovieSessionsCollection}` AS ms2
            SET ms2.isFinished = true
            WHERE META(ms2).id IN (
                SELECT RAW META(ms).id
                FROM `{CouchbaseConstants.MovieSessionsCollection}` AS ms
                JOIN `{CouchbaseConstants.MoviesCollection}` AS m on META(m).id = ms.movieId
                WHERE STR_TO_MILLIS(ms.dateTime) + (m.durationInMinutes * 60000) < STR_TO_MILLIS(NOW_STR())
                AND ms.isFinished = false
            )";

        await scope.QueryAsync<object>(query);
    }

    public async Task<IList<MovieSession>> GetAllAsync(int pageNumber, int pageSize)
    {
        var scope = await _bucketProvider.GetScopeAsync();

        var queryOptions = new QueryOptions()
            .Parameter("offset", (pageNumber - 1) * pageSize)
            .Parameter("pageSize", pageSize);

        var query = $@"
            SELECT META(ms).id,
                   ms.dateTime,
                   ms.movieId,
                   ms.movieHallId,
                   ARRAY_AGG(t) AS Tickets
            FROM `{CouchbaseConstants.MovieSessionsCollection}` AS ms
            LEFT JOIN `{CouchbaseConstants.TicketsCollection}` AS t ON t.movieSessionId = META(ms).id
            WHERE ms.isFinished = false
            GROUP BY ms
            ORDER BY ms.dateTime
            OFFSET $offset
            LIMIT $pageSize";

        var queryResult = await scope.QueryAsync<MovieSession>(query, queryOptions);

        return await queryResult.Rows.ToListAsync();
    }

    public async Task<MovieSession> GetByIdAsync(string id)
    {
        var scope = await _bucketProvider.GetScopeAsync();
        var queryOptions = new QueryOptions().Parameter("id", id);

        var query = $@"
            SELECT META(ms).id,
                   ms.dateTime,
                   ms.movieId,
                   ms.movieHallId,
                   ARRAY_AGG(t) AS Tickets
            FROM `{CouchbaseConstants.MovieSessionsCollection}` AS ms
            LEFT JOIN `{CouchbaseConstants.TicketsCollection}` AS t ON t.movieSessionId = META(ms).id
            WHERE ms.isFinished = false
            GROUP BY ms
            WHERE META(ms).id = $id";

        var queryResult = await scope.QueryAsync<MovieSession>(query, queryOptions);

        return await queryResult.FirstOrDefaultAsync();
    }

    public async Task InsertAsync(MovieSession entity)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.MovieSessionsCollection);
        await collection.InsertAsync(entity.Id.ToString(), entity);
    }

    public async Task UpdateAsync(MovieSession entity)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.MovieSessionsCollection);
        await collection.ReplaceAsync(entity.Id.ToString(), entity);
    }
}
