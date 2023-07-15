using Core.Entities;
using Core.Interfaces.BucketProviders;
using Core.Interfaces.Repositories;

namespace DataAccess.Repositories;

public class TicketsRepository : ITicketsRepository
{
    private readonly IMovieTicketBookingBucketProvider _bucketProvider;

    public TicketsRepository(IMovieTicketBookingBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task Create(Ticket entity)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("tickets");

        await collection.InsertAsync(Guid.NewGuid().ToString(), entity);
    }

    public async Task Delete(Guid id)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("tickets");

        await collection.RemoveAsync(id.ToString());
    }

    public async Task<IList<Ticket>> GetAllAsync(int? pageNumber, int? pageSize)
    {
        var pageIndex = pageNumber.HasValue ? pageNumber.Value : 0;
        var pageLimit = pageSize.HasValue ? pageSize.Value : 5;

        var cluster = (await _bucketProvider.GetBucketAsync()).Cluster;

        var ticketsQuery = await cluster.QueryAsync<Ticket>($@"
            SELECT META(t.Id),
                   t.DateTime,
                   t.SeatNumber,
                   t.MovieSessionId,
                   t.UserId
            FROM tickets AS t
            OFFSET {pageIndex * pageLimit}
            TAKE {pageLimit}");

        return await ticketsQuery.Rows.ToListAsync();
    }

    public async Task<Ticket?> GetByIdAsync(Guid id)
    {
        var cluster = (await _bucketProvider.GetBucketAsync()).Cluster;

        var ticketQuery = await cluster.QueryAsync<Ticket>($@"
            SELECT META(t.Id),
                   t.DateTime,
                   t.SeatNumber,
                   t.MovieSessionId,
                   t.UserId
            FROM tickets AS t
            LIMIT 1");

        return await ticketQuery.Rows.FirstOrDefaultAsync();
    }

    public async Task Update(Ticket entity)
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync("tickets");

        await collection.ReplaceAsync(entity.Id.ToString(), entity);
    }
}
