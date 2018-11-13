using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class UserIdCache : DataDbCache<UserIdCache>, IUserIdCache
    {
        public virtual string Get(UserIdCacheType type, string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            return base.GetData<string>(type, key.ToLower());
        }

        public virtual void Remove(UserIdCacheType type, string key)
        {
            if (!string.IsNullOrEmpty(key)) base.RemoveKey(type, key.ToLower());
        }

        public virtual void Set(UserIdCacheType type, string key, string id)
        {
            if (!string.IsNullOrEmpty(key)) base.SetData(id, type, key.ToLower());
        }
    }
}
