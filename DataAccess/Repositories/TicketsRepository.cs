using Domain.Entities;
using Domain.Extensions;
using Domain.Interfaces.BucketProviders;
using Domain.Interfaces.Repositories;
using Domain.Shared.Constants;
using Couchbase.Query;

namespace DataAccess.Repositories;

public class TicketsRepository : ITicketsRepository
{
	private readonly IMovieTicketBookingBucketProvider _bucketProvider;

	public TicketsRepository(IMovieTicketBookingBucketProvider bucketProvider)
	{
		_bucketProvider = bucketProvider;
	}

	public async Task<IList<Ticket>> GetAllAsync(int pageNumber, int pageSize)
	{
		var scope = await (await _bucketProvider.GetBucketAsync()).ScopeAsync(CouchbaseConstants.DefaultScope);

		var queryOptions = new QueryOptions()
			.Parameter("offset", (pageNumber - 1) * pageSize)
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

		var queryResult = await scope.QueryAsync<Ticket>(query, queryOptions);

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

		var queryOptions = new QueryOptions()
			.Parameter("userId", userId)
			.Parameter("offset", (pageNumber - 1) * pageSize)
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

		var tickets = await scope.QueryAsync<Ticket>(query, queryOptions);

		return await tickets.Rows.ToListAsync();
	}

	public async Task InsertAsync(Ticket ticket)
	{
		var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.TicketsCollection);
		await collection.InsertAsync(ticket.Id.ToString(), ticket);
	}

	public async Task UpdateFinishedAsync()
	{
		var scope = await _bucketProvider.GetScopeAsync();

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
}
