using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.ICache
{
    public interface IUserIdCache : IBaseCache
    {
        string Get(UserIdCacheType type, string key);
        void Set(UserIdCacheType type, string key, string id);
        void Remove(UserIdCacheType type, string key);
    }
}
