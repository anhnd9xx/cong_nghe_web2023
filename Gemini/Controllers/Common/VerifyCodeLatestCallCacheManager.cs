using System;
using System.Runtime.Caching;
using System.Collections.Generic;

namespace Gemini.Controllers.Common
{
    public static class VerifyCodeLatestCallCacheManager
    {
        private static MemoryCache _cache = MemoryCache.Default;

        public static Dictionary<string, DateTime> VerifyCodeLatestCall
        {
            get
            {
                if (!_cache.Contains("VerifyCodeLatestCall"))
                    RefreshVerifyCodeLatestCall("");
                return _cache.Get("VerifyCodeLatestCall") as Dictionary<string, DateTime>;
            }
        }

        public static void RefreshVerifyCodeLatestCall(string phoneNumber)
        {
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddDays(1);

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                VerifyCodeLatestCall[phoneNumber] = DateTime.Now;
                _cache.Set("VerifyCodeLatestCall", VerifyCodeLatestCall, cacheItemPolicy);
            }
            else
            {
                _cache.Set("VerifyCodeLatestCall", new Dictionary<string, DateTime>(), cacheItemPolicy);
            }
        }
    }
}