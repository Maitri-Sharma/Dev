using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.MemoryCache
{
    /// <summary>
    /// IManageCache
    /// </summary>
    public interface IManageCache
    {
        /// <summary>
        /// Gets from cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        Task<T> GetFromCache<T>(string key) where T : class;
        /// <summary>
        /// Sets the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        Task SetCache<T>(string key, T value) where T : class;
    }
}
