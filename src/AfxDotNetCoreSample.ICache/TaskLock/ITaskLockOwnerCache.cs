using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.ICache
{
   public  interface ITaskLockOwnerCache : IBaseCache
    {
        string Get(TaskLockType type, string key);
        void Set(TaskLockType type, string key, string owner);
        void Set(TaskLockType type, string key, string owner, TimeSpan? timeout);
        void Remove(TaskLockType type, string key);
    }
}
