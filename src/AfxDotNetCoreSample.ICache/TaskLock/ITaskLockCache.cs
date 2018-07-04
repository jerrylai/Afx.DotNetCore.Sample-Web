using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.ICache
{
    public interface ITaskLockCache : IBaseCache
    {
        bool Lock(TaskLockType type, string key);

        bool Lock(TaskLockType type, string key, TimeSpan? timeout);

        bool IsLock(TaskLockType type, string key);

        void Release(TaskLockType type, string key);
    }
}
