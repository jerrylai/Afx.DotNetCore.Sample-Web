using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.ICache
{
    public interface IDistributedLockCache : IBaseCache
    {
        bool Lock(LockType type, string key, TimeSpan? timeout);

        bool IsLock(LockType type, string key);

        void Release(LockType type, string key);

        void SetExpire(LockType type, string key, TimeSpan? timeout);
    }
}
