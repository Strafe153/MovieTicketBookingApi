using Core.Shared.Constants;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue;

namespace Core.Extensions;

public static class INamedBucketProviderExtensions
{
    public static async Task<ICouchbaseCollection> GetCollectionAsync(this INamedBucketProvider bucketProvider, string collectionName)
    {
        var bucket = await bucketProvider.GetBucketAsync();
        var collection = await bucket.CollectionAsync(collectionName);

        return collection;
    }

    public static async Task<IScope> GetScopeAsync(
        this INamedBucketProvider bucketProvider,
        string scopeName = CouchbaseConstants.DefaultScope)
    {
        var bucket = await bucketProvider.GetBucketAsync();
        var scope = await bucket.ScopeAsync(scopeName);

        return scope;
    }
}
