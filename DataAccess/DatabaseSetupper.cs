using Core.Entities;
using Core.Interfaces.BucketProviders;
using Core.Shared.Constants;
using Couchbase;
using Couchbase.Management.Collections;
using Couchbase.Management.Query;
using System.Text.Json;
using static Couchbase.Core.Diagnostics.Tracing.OuterRequestSpans.ManagerSpan;

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
        catch (Exception)
        {
            return;
        }
    }

    private static async Task CreateCollectionAsync(
        IBucket bucket,
        string collectionName,
        string scopeName = CouchbaseConstants.DefaultScope) =>
            await bucket.Collections.CreateCollectionAsync(new CollectionSpec(scopeName, collectionName));

    private static async Task CreatePrimaryIndexAsync(IBucket bucket, string collectionName, bool ignoreIfExists = true) =>
        await (await bucket.CollectionAsync(collectionName)).QueryIndexes.CreatePrimaryIndexAsync(
            new CreatePrimaryQueryIndexOptions().IndexName($"idx_{collectionName}_primary").IgnoreIfExists(ignoreIfExists));

    private static async Task CreateJoinIndexAsync(
        IBucket bucket,
        string collectionName,
        IEnumerable<string> fields,
        bool ignoreIfExists = true) =>
            await (await bucket.CollectionAsync(collectionName)).QueryIndexes.CreateIndexAsync(
                $"idx_{collectionName}_{string.Join('_', fields)}",
                fields,
                new CreateQueryIndexOptions().IgnoreIfExists(ignoreIfExists));

    private static async Task ConfigureCollectionsAsync(IBucket bucket) =>
        await Task.WhenAll(CreateCollectionAsync(bucket, CouchbaseConstants.UsersCollection),
            CreateCollectionAsync(bucket, CouchbaseConstants.MoviesCollection),
            CreateCollectionAsync(bucket, CouchbaseConstants.MovieHallsCollection),
            CreateCollectionAsync(bucket, CouchbaseConstants.TicketsCollection),
            CreateCollectionAsync(bucket, CouchbaseConstants.MovieSessionsCollection));

    private static async Task ConfigurePrimaryIndexesAsync(IBucket bucket) =>
        await Task.WhenAll(
            CreatePrimaryIndexAsync(bucket, CouchbaseConstants.UsersCollection),
            CreatePrimaryIndexAsync(bucket, CouchbaseConstants.MoviesCollection),
            CreatePrimaryIndexAsync(bucket, CouchbaseConstants.MovieHallsCollection),
            CreatePrimaryIndexAsync(bucket, CouchbaseConstants.TicketsCollection),
            CreatePrimaryIndexAsync(bucket, CouchbaseConstants.MovieSessionsCollection));

    private static async Task ConfigureJoinIndexesAsync(IBucket bucket) =>
        await Task.WhenAll(
            CreateJoinIndexAsync(
                bucket,
                CouchbaseConstants.MovieSessionsCollection,
                new[]
                {
                    JsonNamingPolicy.CamelCase.ConvertName(nameof(MovieSession.MovieHallId))
                }),
            CreateJoinIndexAsync(
                bucket,
                CouchbaseConstants.MovieSessionsCollection,
                new[]
                {
                    JsonNamingPolicy.CamelCase.ConvertName(nameof(MovieSession.MovieId))
                }),
            CreateJoinIndexAsync(
                bucket,
                CouchbaseConstants.TicketsCollection,
                new[]
                {
                    JsonNamingPolicy.CamelCase.ConvertName(nameof(Ticket.UserId))
                }),
            CreateJoinIndexAsync(
                bucket,
                CouchbaseConstants.TicketsCollection,
                new[]
                {
                    JsonNamingPolicy.CamelCase.ConvertName(nameof(Ticket.MovieSessionId))
                }));
}