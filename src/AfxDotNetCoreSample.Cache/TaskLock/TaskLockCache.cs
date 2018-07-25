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
        public virtual bool IsLock(TaskLockType type, string key)
        {
            string k = base.GetCacheKey(type, key);
            var db = base.GetCacheDb(k);
            var database = RedisUtils.GetDatabase(db);
            var value = database.KeyExists(k);

            return value;
        }

        public virtual bool Lock(TaskLockType type, string key, TimeSpan? timeout)
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

        public virtual void Release(TaskLockType type, string key)
        {
            string k = base.GetCacheKey(type, key);
            var db = base.GetCacheDb(k);
            var database = RedisUtils.GetDatabase(db);
            database.KeyDelete(k);
        }

        public virtual void SetExpire(TaskLockType type, string key, TimeSpan? timeout)
        {
            string k = base.GetCacheKey(type, key);
            var db = base.GetCacheDb(k);
            var database = RedisUtils.GetDatabase(db);
            database.KeyExpire(k, timeout);
        }
    }
}
