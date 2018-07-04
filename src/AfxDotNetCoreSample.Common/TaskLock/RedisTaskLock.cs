using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Common
{
   public  class RedisTaskLock : IDisposable
    {
        public bool IsDisposed { get; private set; } = true;
        public bool IsLockSucceed { get; private set; } = false;
        public TaskLockType Type { get; private set; }
        public string Key { get; private set; }
        public string Owner { get; private set; }

        public RedisTaskLock(TaskLockType type, string key, string owner)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(owner)) throw new ArgumentNullException("owner");
            this.Type = type;
            this.Key = key;
            this.Owner = owner;
            this.IsDisposed = false;
        }

        public bool Lock(TimeSpan? timeout)
        {
            if (this.IsDisposed) throw new ObjectDisposedException("RedisTaskLock");
            var cache = IocUtils.Get<ITaskLockCache>();
            this.IsLockSucceed = cache.Lock(this.Type, this.Key, timeout);
            if (this.IsLockSucceed)
            {
                var ow = IocUtils.Get<ITaskLockOwnerCache>();
                ow.Set(this.Type, this.Key, this.Owner, timeout);
            }

            return this.IsLockSucceed;
        }

        public bool Lock()
        {
            if (this.IsDisposed) throw new ObjectDisposedException("RedisTaskLock");
            var cache = IocUtils.Get<ITaskLockCache>();
            this.IsLockSucceed = cache.Lock(this.Type, this.Key);
            if (this.IsLockSucceed)
            {
                var ow = IocUtils.Get<ITaskLockOwnerCache>();
                ow.Set(this.Type, this.Key, this.Owner);
            }

            return this.IsLockSucceed;
        }

        public bool IsLock()
        {
            if (this.IsDisposed) throw new ObjectDisposedException("RedisTaskLock");
            var cache = IocUtils.Get<ITaskLockCache>();

            return cache.IsLock(this.Type, this.Key);
        }

        public void Release()
        {
            if(this.IsLockSucceed)
            {
                this.IsLockSucceed = false;
                var cache = IocUtils.Get<ITaskLockCache>();
                cache.Release(this.Type, this.Key);
                var ow = IocUtils.Get<ITaskLockOwnerCache>();
                ow.Remove(this.Type, this.Key);
            }
        }

        public void Dispose()
        {
            if(!this.IsDisposed)
            {
                this.IsDisposed = true;
                this.Release();
            }
        }
    }
}
