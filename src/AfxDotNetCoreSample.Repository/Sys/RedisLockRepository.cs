using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;

namespace AfxDotNetCoreSample.Repository
{
    /// <summary>
    /// redis 分布式任务锁实现
    /// </summary>
    public class RedisLockRepository : BaseRepository, IDistributedLockRepository
    {
        protected virtual IDistributedLockCache distributedLockCache => this.GetCache<IDistributedLockCache>();
        protected virtual IDistributedLockOwnerCache distributedLockOwnerCache => this.GetCache<IDistributedLockOwnerCache>();

        /// <summary>
        /// 尝试获取锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空，key长度小于或等于50</param>
        /// <param name="owner">锁定定者，不能为空，owner长度小于或等于50</param>
        /// <param name="timeout">锁超时</param>
        /// <returns></returns>
        public virtual bool Lock(LockType type, string key, string owner, TimeSpan? timeout = null)
        {
            var islock = this.distributedLockCache.Lock(type, key, timeout);
            if (islock)
            {
                this.distributedLockOwnerCache.Set(type, key, owner, timeout);
            }
            else
            {
                var v = this.distributedLockOwnerCache.Get(type, key);
                if (v == owner)//自己锁定
                {
                    islock = true;
                    this.distributedLockCache.SetExpire(type, key, timeout);
                }
            }

            return islock;
        }

        /// <summary>
        /// 查询是否锁定
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key不能为空</param>
        /// <param name="owner">锁定定者，不能为空</param>
        /// <returns></returns>
        public virtual bool IsOtherLock(LockType type, string key, string owner)
        {
            var islock = this.distributedLockCache.IsLock(type, key);
            if (islock)
            {
                var v = this.distributedLockOwnerCache.Get(type, key);
                if (v == owner)//自己锁定
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空</param>
        public virtual void Release(LockType type, string key)
        {
            this.distributedLockCache.Release(type, key);
            this.distributedLockOwnerCache.Remove(type, key);
        }

        /// <summary>
        /// 更新Timeout
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空</param>
        /// <param name="owner">锁定定者，不能为空</param>
        /// <param name="timeout">锁超时</param>
        /// <returns></returns>
        public virtual void UpdateTimeout(LockType type, string key, string owner, TimeSpan? timeout)
        {
            this.distributedLockCache.SetExpire(type, key, timeout);
            this.distributedLockOwnerCache.SetExpire(type, key, timeout);
        }
    }
}
