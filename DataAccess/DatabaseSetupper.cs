using System.Text.Json;
using Couchbase;
using Couchbase.Management.Query;
using Domain.Entities;
using Domain.Interfaces.BucketProviders;
using Domain.Shared.Constants;

namespace DataAccess;

public class DatabaseSetupper
{
    private readonly IMovieTicketBookingBucketProvider _bucketProvider;

    public DatabaseSetupper(IMovieTicketBookingBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task SetupDatabase()
    {
        try
        {
            var bucket = await _bucketProvider.GetBucketAsync();

            await ConfigureCollectionsAsync(bucket);
            await ConfigurePrimaryIndexesAsync(bucket);
            await ConfigureJoinIndexesAsync(bucket);
        }
        catch
        {
            return;
        }
    }

    #region Private methods

    private static Task CreateCollectionAsync(
        IBucket bucket,
        string collectionName,
        string scopeName = CouchbaseConstants.DefaultScope) =>
            bucket.Collections.CreateCollectionAsync(scopeName, collectionName, new());

    private static async Task CreatePrimaryIndexAsync(IBucket bucket, string collectionName, bool ignoreIfExists = true)
    {
        var collection = await bucket.CollectionAsync(collectionName);
        var indexName = $"idx_{collectionName}_primary";

        var index = new CreatePrimaryQueryIndexOptions()
            .IndexName(indexName)
            .IgnoreIfExists(ignoreIfExists);

        await collection.QueryIndexes.CreatePrimaryIndexAsync(index);
    }

    private static async Task CreateJoinIndexAsync(
        IBucket bucket,
        string collectionName,
        IEnumerable<string> fields,
        bool ignoreIfExists = true)
    {
        var collection = await bucket.CollectionAsync(collectionName);
        var index = new CreateQueryIndexOptions().IgnoreIfExists(ignoreIfExists);

        var joinedFields = string.Join('_', fields);
        var indexName = $"idx_{collectionName}_{joinedFields}";

        await collection.QueryIndexes.CreateIndexAsync(indexName, fields, index);
    }

    private static Task ConfigureCollectionsAsync(IBucket bucket)
    {
        Task[] collectionTasks = [
            CreateCollectionAsync(bucket, CouchbaseConstants.UsersCollection),
            CreateCollectionAsync(bucket, CouchbaseConstants.MoviesCollection),
            CreateCollectionAsync(bucket, CouchbaseConstants.MovieHallsCollection),
            CreateCollectionAsync(bucket, CouchbaseConstants.TicketsCollection),
            CreateCollectionAsync(bucket, CouchbaseConstants.MovieSessionsCollection)
        ];

        return Task.WhenAll(collectionTasks);
    }

    private static Task ConfigurePrimaryIndexesAsync(IBucket bucket)
    {
        Task[] indexTasks = [
            CreatePrimaryIndexAsync(bucket, CouchbaseConstants.UsersCollection),
            CreatePrimaryIndexAsync(bucket, CouchbaseConstants.MoviesCollection),
            CreatePrimaryIndexAsync(bucket, CouchbaseConstants.MovieHallsCollection),
            CreatePrimaryIndexAsync(bucket, CouchbaseConstants.TicketsCollection),
            CreatePrimaryIndexAsync(bucket, CouchbaseConstants.MovieSessionsCollection)
        ];

        return Task.WhenAll(indexTasks);
    }

    private static Task ConfigureJoinIndexesAsync(IBucket bucket)
    {
        Task[] joinIndexTasks = [
            CreateJoinIndexAsync(
                bucket,
                CouchbaseConstants.MovieSessionsCollection,
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(MovieSession.MovieHallId))]),
            CreateJoinIndexAsync(
                bucket,
                CouchbaseConstants.MovieSessionsCollection,
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(MovieSession.MovieId))]),
            CreateJoinIndexAsync(
                bucket,
                CouchbaseConstants.TicketsCollection,
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(Ticket.UserId))]),
            CreateJoinIndexAsync(
                bucket,
                CouchbaseConstants.TicketsCollection,
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(Ticket.MovieSessionId))])
            ];

        return Task.WhenAll(joinIndexTasks);
    }

    #endregion
}