using Domain.Entities;
using Domain.Extensions;
using Domain.Interfaces.BucketProviders;
using Domain.Interfaces.Repositories;
using Domain.Shared.Constants;
using Couchbase.KeyValue;
using Couchbase.Query;
using System.Text.Json;

namespace DataAccess.Repositories;

public class MovieHallsRepository : IMovieHallsRepository
{
	private readonly IMovieTicketBookingBucketProvider _bucketProvider;

	public MovieHallsRepository(IMovieTicketBookingBucketProvider bucketProvider)
	{
		_bucketProvider = bucketProvider;
	}

	public async Task DeleteAsync(string id)
	{
		var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.MovieHallsCollection);

		await collection.MutateInAsync(id, specs =>
			specs.Upsert(JsonNamingPolicy.CamelCase.ConvertName(nameof(MovieHall.IsActive)), false));
	}

	public async Task<IList<MovieHall>> GetAllAsync(int pageNumber, int pageSize)
	{
		var scope = await _bucketProvider.GetScopeAsync();

		var queryOptions = new QueryOptions()
			.Parameter("offset", (pageNumber - 1) * pageSize)
			.Parameter("pageSize", pageSize);

		var query = $@"
            SELECT META(mh).id,
                   mh.name,
                   mh.numberOfSeats,
                   ARRAY_AGG(ms) AS MovieSessions
            FROM `{CouchbaseConstants.MovieHallsCollection}` AS mh
            LEFT JOIN `{CouchbaseConstants.MovieSessionsCollection}` AS ms ON ms.movieHallId = META(mh).id
            GROUP BY mh
            ORDER BY mh.name, mh.numberOfSeats
            OFFSET $offset
            LIMIT $pageSize";

		var queryResult = await scope.QueryAsync<MovieHall>(query, queryOptions);

		return await queryResult.Rows.ToListAsync();
	}

	public async Task<MovieHall> GetByIdAsync(string id)
	{
		var scope = await _bucketProvider.GetScopeAsync();
		var queryOptions = new QueryOptions().Parameter("id", id);

		var query = $@"
            SELECT META(mh).id,
                   mh.name,
                   mh.numberOfSeats,
                   ARRAY_AGG(ms) AS MovieSessions
            FROM `{CouchbaseConstants.MovieHallsCollection}` AS mh
            LEFT JOIN `{CouchbaseConstants.MovieSessionsCollection}` AS ms ON ms.movieHallId = META(mh).id
            GROUP BY mh
            WHERE META(mh).id = $id";

		var queryResult = await scope.QueryAsync<MovieHall>(query, queryOptions);

		return await queryResult.FirstOrDefaultAsync();
	}

	public async Task InsertAsync(MovieHall movieHall)
	{
		var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.MovieHallsCollection);
		await collection.InsertAsync(movieHall.Id.ToString(), movieHall);
	}

	public async Task UpdateAsync(MovieHall movieHall)
	{
		var collection = await _bucketProvider.GetCollectionAsync(CouchbaseConstants.MovieHallsCollection);
		await collection.ReplaceAsync(movieHall.Id.ToString(), movieHall);
	}
}
