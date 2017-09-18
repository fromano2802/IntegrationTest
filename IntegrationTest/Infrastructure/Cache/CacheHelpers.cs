using System.Linq;
using LazyCache;

namespace IntegrationTest.Infrastructure
{
    public static class CacheHelpers
    {
        public static void ClearCache(this IAppCache cache)
        {
            var cacheKeys = cache.ObjectCache.Select(kvp => kvp.Key);
            foreach (var cacheKey in cacheKeys)
            {
                cache.ObjectCache.Remove(cacheKey);
            }
        }
    }
}