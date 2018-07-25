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
            return base.GetData<string>(type, key);
        }

        public virtual void Remove(TaskLockType type, string key)
        {
            base.RemoveKey(type, key);
        }
        
        public virtual void Set(TaskLockType type, string key, string owner, TimeSpan? timeout)
        {
            base.SetData(owner, type, key, timeout);
        }

        public virtual void SetExpire(TaskLockType type, string key, TimeSpan? timeout)
        {
            base.SetDataExpire(timeout, type, key);
        }
    }
}
