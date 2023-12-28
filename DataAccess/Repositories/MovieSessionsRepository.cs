using Core.Entities;
using Core.Extensions;
using Core.Interfaces.BucketProviders;
using Core.Interfaces.Repositories;
using Core.Shared.Constants;
using Couchbase.KeyValue;
using System.Text.Json;

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

    public async Task DeleteFinished()
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.MovieSessionsCollection);
        var scope = await _bucketProvider.GetScopeAsync();
        //var query = $@"
        //    SELECT META(ms).id,
        //           ms.dateTime,
        //           m.durationInMinutes
        //    FROM `{CouchbaseConstants.MOVIE_SESSIONS_COLLECTION}` AS ms
        //    JOIN `{CouchbaseConstants.MOVIES_COLLECTION}` AS m ON ms.movieId = META(m).id
        //    WHERE ms.hasFinished = 0";
        var query = $@"
            SELECT META(ms).id AS Item1,
                   ms.dateTime AS Item2,
                   m.durationInMinutes AS Item3
            FROM `{CouchbaseConstants.MovieSessionsCollection}` AS ms
            JOIN `{CouchbaseConstants.MoviesCollection}` AS m ON ms.movieId = META(m).id
            WHERE ms.hasFinished = false";

        //var queryResult = await scope.QueryAsync<MovieSession>(query);
        var queryResult = await scope.QueryAsync<(Guid, DateTime, int)>(query);
        var items = await queryResult.Rows.ToListAsync();

        foreach (var item in items)
        {
            if (item.Item2.AddMinutes(item.Item3) > DateTime.Now)
            {
                await collection.MutateInAsync(
                    item.Item1.ToString(),
                    specs => specs.Upsert(JsonNamingPolicy.CamelCase.ConvertName(nameof(MovieSession.HasFinished)), true));
            }
        }
    }

    public async Task<IList<MovieSession>> GetAllAsync(int pageNumber, int pageSize)
    {
        var scope = await _bucketProvider.GetScopeAsync();
        var query = $@"
            SELECT META(ms).id,
                   ms.dateTime,
                   ms.movieId,
                   ms.movieHallId,
                   ARRAY_AGG(t) AS Tickets
            FROM `{CouchbaseConstants.MovieSessionsCollection}` AS ms
            LEFT JOIN `{CouchbaseConstants.TicketsCollection}` AS t ON t.movieSessionId = META(ms).id
            WHERE ms.hasFinished = false
            GROUP BY ms
            ORDER BY ms.dateTime
            OFFSET {(pageNumber - 1) * pageSize}
            LIMIT {pageSize}";

        var queryResult = await scope.QueryAsync<MovieSession>(query);

        return await queryResult.Rows.ToListAsync();
    }

    public async Task<MovieSession> GetByIdAsync(string id)
    {
        var scope = await _bucketProvider.GetScopeAsync();
        var query = $@"
            SELECT META(ms).id,
                   ms.dateTime,
                   ms.movieId,
                   ms.movieHallId,
                   ARRAY_AGG(t) AS Tickets
            FROM `{CouchbaseConstants.MovieSessionsCollection}` AS ms
            LEFT JOIN `{CouchbaseConstants.TicketsCollection}` AS t ON t.movieSessionId = META(ms).id
            WHERE ms.hasFinished = 0
            GROUP BY ms
            WHERE META(ms).id = 'id'";

        var queryResult = await scope.QueryAsync<MovieSession>(query);

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
