using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Afx.Cache;
using Afx.Ioc;

namespace AfxDotNetCoreSample.Common
{
    public class CacheKeyUtils
    {
        private static CacheKey _cacheKey;
        public static CacheKey cacheKey
        {
            get
            {
                if (_cacheKey == null)
                {
                    _cacheKey = IocUtils.GetSingle<CacheKey>();
                }
                return _cacheKey;
            }
        }

        public static List<int> GetDb(string node, string name)
        {
            return cacheKey.GetDb(node, name);
        }

        public static string GetKey(string node, string name)
        {
            var key = cacheKey.GetKey(node, name);
            if (!string.IsNullOrEmpty(key)) key = $"{ConfigUtils.CachePrefix}{key}";

            return key;
        }

        public static TimeSpan? GetExpire(string node, string name)
        {
            return cacheKey.GetExpire(node, name);
        }
    }
}
