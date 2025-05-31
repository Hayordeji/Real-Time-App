using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task<T?> GetData<T>(string key)
        {
            var data = await _cache.GetStringAsync(key);

            if (data is null)
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(data);
        }

        public Task SetData<T>(string key, T value)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3) // Set expiration time as needed
            };
            var data = JsonConvert.SerializeObject(value);
            return _cache.SetStringAsync(key, data, options);
        }
    }
}
