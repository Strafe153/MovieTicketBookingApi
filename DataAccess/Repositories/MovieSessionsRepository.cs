using Couchbase.KeyValue;
using Couchbase.Query;
using Domain.Entities;
using Domain.Extensions;
using Domain.Interfaces.BucketProviders;
using Domain.Interfaces.Repositories;
using Domain.Shared.Constants;

namespace DataAccess.Repositories;

public class MovieSessionsRepository(IMovieTicketBookingBucketProvider bucketProvider)
    : Repository(bucketProvider), IMovieSessionsRepository
{
    public async Task UpdateFinishedAsync()
    {
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

        var scope = await GetScopeAsync();
        await scope.QueryAsync<object>(query);
    }

    public async Task<IList<MovieSession>> GetAllAsync(int pageNumber, int pageSize)
    {
        var offset = GetOffset(pageNumber, pageSize);

        var queryOptions = new QueryOptions()
            .Parameter("offset", offset)
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

        var scope = await GetScopeAsync();
        var queryResult = await scope.QueryAsync<MovieSession>(query, queryOptions);
        var movieSessions = await queryResult.Rows.ToListAsync();

        return movieSessions;
    }

    public async Task<MovieSession> GetByIdAsync(string id)
    {
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

        var scope = await GetScopeAsync();
        var queryResult = await scope.QueryAsync<MovieSession>(query, queryOptions);
        var movieSession = await queryResult.FirstOrDefaultAsync();

        return movieSession;
    }

    public async Task InsertAsync(MovieSession movieSession)
    {
        var collection = await GetCollectionAsync();
        await collection.InsertAsync(movieSession.Id.ToString(), movieSession);
    }

    public async Task UpdateAsync(MovieSession movieSession)
    {
        var collection = await GetCollectionAsync();
        await collection.ReplaceAsync(movieSession.Id.ToString(), movieSession);
    }

    protected override Task<ICouchbaseCollection> GetCollectionAsync() =>
        _bucketProvider.GetCollectionAsync(CouchbaseConstants.MovieSessionsCollection);
}
