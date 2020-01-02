using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWithSwagger.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache distributedCache;

        public ResponseCacheService(IDistributedCache _distributedCache)
        {
            distributedCache = _distributedCache;
        }
        public async Task CachedResponseAsync(string cacheKey, object response, TimeSpan timeTimeLive)
        {

            if(response == null)
            {
                return;
            }

            var serializedResponse = JsonConvert.SerializeObject(response);
            var distributedCacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeTimeLive
            };
            await distributedCache.SetStringAsync(cacheKey,serializedResponse, distributedCacheOptions);
        }

        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            string cacheResponse = String.Empty;
            try
            {
                cacheResponse = await distributedCache.GetStringAsync(cacheKey);

            }
            catch(Exception ex)
            {
                var msg = ex.Message;
            }
            

            return string.IsNullOrEmpty(cacheResponse) ? null : cacheResponse;
        }
    }
}
