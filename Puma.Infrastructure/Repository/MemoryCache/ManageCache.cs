using Microsoft.Extensions.Caching.Memory;
using Puma.Infrastructure.Interface.KsupDB.Reol;
using Puma.Infrastructure.Interface.MemoryCache;
using Puma.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Repository.MemoryCache
{
    /// <summary>
    /// ManageCache
    /// </summary>
    /// <seealso cref="Puma.Infrastructure.Interface.MemoryCache.IManageCache" />
    public class ManageCache : IManageCache
    {
        /// <summary>
        /// The memory cache
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageCache"/> class.
        /// </summary>
        /// <param name="memoryCache">The memory cache.</param>
        /// <exception cref="System.ArgumentNullException">memoryCache</exception>
        public ManageCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        /// <summary>
        /// Gets from cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public async Task<T> GetFromCache<T>(string key) where T : class
        {
            await Task.Run(() => { });
            _memoryCache.TryGetValue(key, out T cachedResponse);
            return cachedResponse as T;
        }

        /// <summary>
        /// Sets the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public async Task SetCache<T>(string key, T value) where T : class
        {
            await Task.Run(() => { });
            _memoryCache.Set(key, value);
        }
    }
}
