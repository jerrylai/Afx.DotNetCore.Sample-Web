using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.Common;

namespace AfxDotNetCoreSample.Cache
{
    public class TaskLockCache : TaskLockDbCache, ITaskLockCache
    {
        public bool IsLock(TaskLockType type, string key)
        {
            var value = base.Get<long>(type, key);

            return value > 0;
        }

        public bool Lock(TaskLockType type, string key)
        {
            var expire = base.GetConfigExpire();

            return this.Lock(type, key, expire);
        }

        public bool Lock(TaskLockType type, string key, TimeSpan? timeout)
        {
            string k = base.GetCacheKey(type, key);
            var db = base.GetCacheDb(k);
            var database = RedisUtils.GetDatabase(db);
            var value = database.StringIncrement(k);
            if (value == 1L)
            {
                database.KeyExpire(k, timeout);
                return true;
            }

            return false;
        }

        public void Release(TaskLockType type, string key)
        {
            base.Remove(type, key);
        }
    }
}
