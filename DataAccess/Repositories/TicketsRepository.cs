using Couchbase.KeyValue;
using Couchbase.Query;
using Domain.Entities;
using Domain.Extensions;
using Domain.Interfaces.BucketProviders;
using Domain.Interfaces.Repositories;
using Domain.Shared.Constants;

namespace DataAccess.Repositories;

public class TicketsRepository(IMovieTicketBookingBucketProvider bucketProvider)
    : Repository(bucketProvider), ITicketsRepository
{
    public async Task<IList<Ticket>> GetAllAsync(int pageNumber, int pageSize)
    {
        var offset = GetOffset(pageNumber, pageSize);

        var queryOptions = new QueryOptions()
            .Parameter("offset", offset)
            .Parameter("pageSize", pageSize);

        var query = $@"
            SELECT META(t).id,
                   t.dateTime,
                   t.seatNumber,
                   t.movieSessionId,
                   t.userId
            FROM `{CouchbaseConstants.TicketsCollection}` AS t
            WHERE t.isCompleted = false
            OFFSET $offset
            LIMIT $pageSize";

        var scope = await GetDefaultScopeAsync();
        var queryResult = await scope.QueryAsync<Ticket>(query, queryOptions);
        var tickets = await queryResult.Rows.ToListAsync();

        return tickets;
    }

    public async Task<Ticket> GetByIdAsync(string id)
    {
        var collection = await GetCollectionAsync();
        var getResult = await collection.GetAsync(id);
        var ticket = getResult.ContentAs<Ticket>();

        if (ticket is not null)
        {
            ticket.Id = Guid.Parse(id);
        }

        return ticket!;
    }

    public async Task<IList<Ticket>> GetByUserIdAsync(int pageNumber, int pageSize, string userId)
    {
        var offset = GetOffset(pageNumber, pageSize);

        var queryOptions = new QueryOptions()
            .Parameter("userId", userId)
            .Parameter("offset", offset)
            .Parameter("pageSize", pageSize);

        var query = $@"
            SELECT META(t).id,
                   t.dateTime,
                   t.seatNumber,
                   t.movieSessionId,
                   t.userId
            FROM `{CouchbaseConstants.TicketsCollection}` AS t
            WHERE t.userId = $userId
                  AND t.isCompleted = false
            ORDER BY t.dateTime, t.seatNumber
            OFFSET $offset
            LIMIT $pageSize";

        var scope = await GetScopeAsync();
        var queryResult = await scope.QueryAsync<Ticket>(query, queryOptions);
        var tickets = await queryResult.Rows.ToListAsync();

        return tickets;
    }

    public async Task InsertAsync(Ticket ticket)
    {
        var collection = await GetCollectionAsync();
        await collection.InsertAsync(ticket.Id.ToString(), ticket);
    }

    public async Task UpdateFinishedAsync()
    {
        var scope = await GetScopeAsync();

        var query = $@"
            UPDATE `{CouchbaseConstants.TicketsCollection}` AS t
            SET t.isCompleted = true
            WHERE META(t).movieSessionsId IN (
                SELECT RAW META(ms).id
                FROM `{CouchbaseConstants.MovieSessionsCollection}` AS ms
                JOIN `{CouchbaseConstants.MoviesCollection}` AS m on META(m).id = ms.movieId
                WHERE STR_TO_MILLIS(ms.dateTime) + (m.durationInMinutes * 60000) < STR_TO_MILLIS(NOW_STR())
                AND ms.isFinished = false
            )";

        await scope.QueryAsync<object>(query);
    }

    protected override Task<ICouchbaseCollection> GetCollectionAsync() =>
        _bucketProvider.GetCollectionAsync(CouchbaseConstants.TicketsCollection);
}
