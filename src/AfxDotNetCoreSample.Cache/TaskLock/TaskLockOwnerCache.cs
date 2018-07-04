using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class TaskLockOwnerCache : TaskLockDbCache, ITaskLockOwnerCache
    {
        public virtual string Get(TaskLockType type, string key)
        {
            return base.Get<string>(type, key);
        }

        public virtual void Remove(TaskLockType type, string key)
        {
            base.Remove(type, key);
        }

        public virtual void Set(TaskLockType type, string key, string owner)
        {
            base.Set(owner, type, key);
        }

        public virtual void Set(TaskLockType type, string key, string owner, TimeSpan? timeout)
        {
            base.Set(owner, type, key, timeout);
        }
    }
}
