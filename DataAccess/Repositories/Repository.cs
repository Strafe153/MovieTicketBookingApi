using Couchbase.KeyValue;
using Domain.Extensions;
using Domain.Interfaces.BucketProviders;
using Domain.Shared.Constants;

namespace DataAccess.Repositories
{
    public abstract class Repository
    {
        protected readonly IMovieTicketBookingBucketProvider _bucketProvider;

        protected Repository(IMovieTicketBookingBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        protected abstract Task<ICouchbaseCollection> GetCollectionAsync();

        protected async Task<IScope> GetDefaultScopeAsync()
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var scope = await bucket.ScopeAsync(CouchbaseConstants.DefaultScope);

            return scope;
        }

        protected Task<IScope> GetScopeAsync() => _bucketProvider.GetScopeAsync();

        protected static int GetOffset(int pageNumber, int pageSize) => (pageNumber - 1) * pageSize;
    }
}
