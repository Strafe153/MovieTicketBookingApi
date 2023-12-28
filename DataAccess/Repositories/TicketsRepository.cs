using Core.Entities;
using Core.Extensions;
using Core.Interfaces.BucketProviders;
using Core.Interfaces.Repositories;
using Core.Shared.Constants;

namespace DataAccess.Repositories;

public class TicketsRepository : ITicketsRepository
{
    private readonly IMovieTicketBookingBucketProvider _bucketProvider;

    public TicketsRepository(IMovieTicketBookingBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task DeleteAsync(string id)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.TicketsCollection);
        await collection.RemoveAsync(id);
    }

    public async Task<IList<Ticket>> GetAllAsync(int pageNumber, int pageSize)
    {
        var scope = await (await _bucketProvider.GetBucketAsync()).ScopeAsync(CouchbaseConstants.DefaultScope);
        var query = $@"
            SELECT META(t).id,
                   t.dateTime,
                   t.seatNumber,
                   t.movieSessionId,
                   t.userId
            FROM `{CouchbaseConstants.TicketsCollection}` AS t
            OFFSET {(pageNumber - 1) * pageSize}
            LIMIT {pageSize}";

        var queryResult = await scope.QueryAsync<Ticket>(query);

        return await queryResult.Rows.ToListAsync();
    }

    public async Task<Ticket> GetByIdAsync(string id)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.TicketsCollection);
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
        var scope = await _bucketProvider.GetScopeAsync();
        var query = $@"
            SELECT META(t).id,
                   t.dateTime,
                   t.seatNumber,
                   t.movieSessionId,
                   t.userId
            FROM `{CouchbaseConstants.TicketsCollection}` AS t
            WHERE t.userId = '{userId}'
            ORDER BY t.dateTime, t.seatNumber
            OFFSET {(pageNumber - 1) * pageSize}
            LIMIT {pageSize}";

        var tickets = await scope.QueryAsync<Ticket>(query);

        return await tickets.Rows.ToListAsync();
    }

    public async Task InsertAsync(Ticket entity)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.TicketsCollection);
        await collection.InsertAsync(entity.Id.ToString(), entity);
    }

    public async Task UpdateAsync(Ticket entity)
    {
        var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.TicketsCollection);
        await collection.ReplaceAsync(entity.Id.ToString(), entity);
    }
}
