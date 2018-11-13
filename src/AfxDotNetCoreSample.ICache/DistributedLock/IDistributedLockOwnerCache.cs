using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.ICache
{
   public  interface IDistributedLockOwnerCache : IBaseCache
    {
        string Get(LockType type, string key);
        void Set(LockType type, string key, string owner, TimeSpan? timeout);
        void Remove(LockType type, string key);
        void SetExpire(LockType type, string key, TimeSpan? timeout);
    }
}
