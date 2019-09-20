using Ask_Alfred.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MemoryCache = Microsoft.Extensions.Caching.Memory.MemoryCache;

namespace Ask_Alfred.Infrastructure
{
    public class MemoryCacheIPage
    {
        private MemoryCache m_Cache = new MemoryCache(new MemoryCacheOptions());
        private ConcurrentDictionary<object, SemaphoreSlim> m_Locks = new ConcurrentDictionary<object, SemaphoreSlim>();

        public async Task<IPage> GetOrCreate(object key, Func<Task<IPage>> createItem)
        {
            IPage cacheEntry;

            if (!m_Cache.TryGetValue(key, out cacheEntry))
            {
                SemaphoreSlim mylock = m_Locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));

                await mylock.WaitAsync();
                try
                {
                    if (!m_Cache.TryGetValue(key, out cacheEntry))
                    {
                        cacheEntry = await createItem();
                        m_Cache.Set(key, cacheEntry);
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            return cacheEntry;
        }
    }
}
