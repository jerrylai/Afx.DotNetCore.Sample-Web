using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class DistributedLockOwnerCache : DistributedLockDbCache<DistributedLockOwnerCache>, IDistributedLockOwnerCache
    {
        public virtual string Get(LockType type, string key)
        {
            return base.GetData<string>(type, key);
        }

        public virtual void Remove(LockType type, string key)
        {
            base.RemoveKey(type, key);
        }
        
        public virtual void Set(LockType type, string key, string owner, TimeSpan? timeout)
        {
            base.SetData(owner, type, key, timeout);
        }

        public virtual void SetExpire(LockType type, string key, TimeSpan? timeout)
        {
            base.SetDataExpire(timeout, type, key);
        }
    }
}
