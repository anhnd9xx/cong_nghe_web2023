using System;
using System.Runtime.Caching;
using System.Collections.Generic;

namespace Gemini.Controllers.Common
{
    public static class TokenCacheManager
    {
        private static MemoryCache _cache = MemoryCache.Default;

        public static Dictionary<string, string> TokenCached
        {
            get
            {
                if (!_cache.Contains("TokenCached"))
                    RefreshTokenCached("", "");
                return _cache.Get("TokenCached") as Dictionary<string, string>;
            }
        }

        public static void RefreshTokenCached(string phoneNumber, string tokenCached)
        {
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddDays(1);

            if (!string.IsNullOrWhiteSpace(phoneNumber) && !string.IsNullOrWhiteSpace(tokenCached))
            {
                TokenCached[phoneNumber] = tokenCached;
                _cache.Set("TokenCached", TokenCached, cacheItemPolicy);
            }
            else
            {
                _cache.Set("TokenCached", new Dictionary<string, string>(), cacheItemPolicy);
            }
        }
    }
}