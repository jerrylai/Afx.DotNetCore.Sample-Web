using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.Common;

namespace AfxDotNetCoreSample.Service
{
    /// <summary>
    /// 分布式锁
    /// </summary>
    public class SyncLock : ISyncLock
    {
        private Lazy<ITaskLockService> service = new Lazy<ITaskLockService>(() => IocUtils.Get<ITaskLockService>(), false);

        public TaskLockType Type { get; private set; }

        public string Key { get; private set; }
        
        public string Owner { get; private set; }
        
        public TimeSpan? Timeout { get; private set; }
        
        public bool IsLockSucceed { get; private set; }

        public SyncLock(TaskLockType type, string key, string owner = null,
            TimeSpan? timeout = null)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrEmpty(owner)) owner = Guid.NewGuid().ToString("n");
            this.Type = type;
            this.Key = key;
            this.Owner = owner;
            this.Timeout = timeout;
            
            this.IsLockSucceed = false;
        }

        public bool IsOtherLock()
        {
            if (this.IsLockSucceed) return true;
            var result = this.service.Value.IsOtherLock(this.Type, this.Key, this.Owner);

            return result;
        }

        public bool Lock()
        {
            if (this.IsLockSucceed) throw new MethodAccessException("不能重复锁！");
            this.IsLockSucceed = this.service.Value.Lock(this.Type, this.Key, this.Owner, this.Timeout);

            return this.IsLockSucceed;
        }

        public void Release()
        {
            if (!this.IsLockSucceed) return;
            this.service.Value.Release(this.Type, this.Key);
            this.IsLockSucceed = false;
        }

        public void UpdateTimeout()
        {
            if (!this.IsLockSucceed && !this.Timeout.HasValue) return;
            this.service.Value.UpdateTimeout(this.Type, this.Key, this.Owner, this.Timeout);
        }

        public void Dispose()
        {
            this.Release();
        }
    }
}
