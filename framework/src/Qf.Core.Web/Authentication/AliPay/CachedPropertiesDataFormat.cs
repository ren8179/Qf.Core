using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using System;

namespace Qf.Core.Web.Authentication.AliPay
{
    /// <summary>
    /// 自定义缓存PropertiesDataFormat
    /// https://github.com/IdentityServer/IdentityServer4/issues/407
    /// </summary>
    public class CachedPropertiesDataFormat
        : ISecureDataFormat<AuthenticationProperties>
    {
        public const string CacheKeyPrefix = "CachedPropertiesData-";

        private readonly IDistributedCache _cache;
        private readonly IDataProtector _dataProtector;
        private readonly IDataSerializer<AuthenticationProperties> _serializer;

        public CachedPropertiesDataFormat(
            IDistributedCache cache,
            IDataProtector dataProtector)
            : this(cache, dataProtector, new PropertiesSerializer())
        {
        }

        public CachedPropertiesDataFormat(
            IDistributedCache cache,
            IDataProtector dataProtector,
            IDataSerializer<AuthenticationProperties> serializer)
        {
            _dataProtector = dataProtector;
            _cache = cache;
            _serializer = serializer;
        }

        public string Protect(AuthenticationProperties data)
        {
            return Protect(data, null);
        }

        public string Protect(AuthenticationProperties data, string purpose)
        {
            var key = Guid.NewGuid().ToString();
            var cacheKey = $"{CacheKeyPrefix}{key}";
            var serialized = _serializer.Serialize(data);

            // Rather than encrypt the full AuthenticationProperties
            // cache the data and encrypt the key that points to the data
            _cache.Set(cacheKey, serialized, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(1) });

            return key;
        }

        public AuthenticationProperties Unprotect(string protectedText)
        {
            return Unprotect(protectedText, null);
        }

        public AuthenticationProperties Unprotect(string protectedText, string purpose)
        {
            // Decrypt the key and retrieve the data from the cache.
            var key = protectedText;
            var cacheKey = $"{CacheKeyPrefix}{key}";
            var serialized = _cache.Get(cacheKey);
            if (serialized == null) return null;
            return _serializer.Deserialize(serialized);
        }

    }
}
