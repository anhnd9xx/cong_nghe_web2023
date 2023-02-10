using System;
using System.Runtime.Caching;
using System.Collections.Generic;

namespace Gemini.Controllers.Common
{
    public static class VerifyCodeCacheManager
    {
        private static MemoryCache _cache = MemoryCache.Default;

        public static Dictionary<string, string> VerifyCode
        {
            get
            {
                if (!_cache.Contains("VerifyCode"))
                    RefreshVerifyCode("", "");
                return _cache.Get("VerifyCode") as Dictionary<string, string>;
            }
        }

        public static void RefreshVerifyCode(string phoneNumber, string verifyCode)
        {
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddDays(1);

            if (!string.IsNullOrWhiteSpace(phoneNumber) && !string.IsNullOrWhiteSpace(verifyCode))
            {
                VerifyCode[phoneNumber] = verifyCode;
                _cache.Set("VerifyCode", VerifyCode, cacheItemPolicy);
            }
            else
            {
                _cache.Set("VerifyCode", new Dictionary<string, string>(), cacheItemPolicy);
            }
        }
    }
}