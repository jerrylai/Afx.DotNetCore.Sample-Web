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
        internal protected virtual ITaskLockService taskLockService => service.Value;

        public LockType Type { get; private set; }

        public string Key { get; private set; }
        
        public string Owner { get; private set; }
        
        public TimeSpan? Timeout { get; private set; }
        
        public bool IsLockSucceed { get; private set; }

        private bool isInit = false;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="owner"></param>
        /// <param name="timeout"></param>
        public virtual void Init(LockType type, string key, string owner = null,
            TimeSpan? timeout = null)
        {
            if (this.isInit) throw new InvalidOperationException("不能重复调用初始化方法！");
            if (string.IsNullOrEmpty(key)) throw new ApiParamNullException(nameof(key));
            if (string.IsNullOrEmpty(owner)) owner = Guid.NewGuid().ToString("n");
            this.Type = type;
            this.Key = key;
            this.Owner = owner;
            this.Timeout = timeout;
            this.IsLockSucceed = false;
            this.isInit = true;
        }

        private void CheckInit()
        {
            if (!this.isInit) throw new InvalidOperationException("未初始化！");
        }

        public bool IsOtherLock()
        {
            this.CheckInit();
            if (this.IsLockSucceed) return true;
            var result = this.taskLockService.IsOtherLock(this.Type, this.Key, this.Owner);

            return result;
        }

        public bool Lock()
        {
            this.CheckInit();
            if (this.IsLockSucceed) throw new MethodAccessException("不能重复锁！");
            this.IsLockSucceed = this.taskLockService.Lock(this.Type, this.Key, this.Owner, this.Timeout);

            return this.IsLockSucceed;
        }

        public void Release()
        {
            this.CheckInit();
            if (!this.IsLockSucceed) return;
            this.taskLockService.Release(this.Type, this.Key);
            this.IsLockSucceed = false;
        }

        public void UpdateTimeout()
        {
            this.CheckInit();
            if (!this.IsLockSucceed && !this.Timeout.HasValue) return;
            this.taskLockService.UpdateTimeout(this.Type, this.Key, this.Owner, this.Timeout);
        }

        public void Dispose()
        {
            if (this.isInit)
            {
                this.Release();
            }
        }
    }
}
