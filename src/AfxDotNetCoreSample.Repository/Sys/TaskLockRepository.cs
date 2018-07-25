using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Models;
using AfxDotNetCoreSample.Common;
using Afx.Utils;

namespace AfxDotNetCoreSample.Repository
{
    /// <summary>
    /// 分布式任务锁实现
    /// </summary>
    public class TaskLockRepository : BaseRepository, ITaskLockRepository
    {
        private Lazy<ITaskLockCache> _taskLockCache = new Lazy<ITaskLockCache>(()=> IocUtils.Get<ITaskLockCache>(), false);
        private Lazy<ITaskLockOwnerCache> _taskLockOwnerCache = new Lazy<ITaskLockOwnerCache>(() => IocUtils.Get<ITaskLockOwnerCache>(), false);
        private ITaskLockCache taskLockCache => _taskLockCache.Value;
        private ITaskLockOwnerCache taskLockOwnerCache => _taskLockOwnerCache.Value;

        /// <summary>
        /// 尝试获取锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空，key长度小于或等于50</param>
        /// <param name="owner">锁定定者，不能为空，owner长度小于或等于50</param>
        /// <param name="timeout">锁超时</param>
        /// <param name="syncLockType">SyncLockType</param>
        /// <returns></returns>
        public virtual bool Lock(TaskLockType type, string key, string owner, TimeSpan? timeout= null, SyncLockType syncLockType = SyncLockType.Database)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (key.Length > 50) throw new ArgumentOutOfRangeException(nameof(key), "Length不能大于50！");
            if (string.IsNullOrEmpty(owner)) throw new ArgumentNullException(nameof(owner));
            if (owner.Length > 50) throw new ArgumentOutOfRangeException(nameof(owner), "Length不能大于50！");
            if (ConfigUtils.CacheType != CacheType.Redis) throw new NotSupportedException($"系统缓存类型(ConfigUtils.CacheType={ConfigUtils.CacheType})不支持 SyncLockType={syncLockType}!");
            switch (syncLockType)
            {
                case SyncLockType.Database:
                    return this.DbLock(type, key, owner, timeout);
                case SyncLockType.Redis:
                    return this.RedisLock(type, key, owner, timeout);
                default:
                    throw new NotImplementedException($"SyncLockType: {syncLockType} 未实现！");
            }
        }

        private bool DbLock(TaskLockType type, string key, string owner, TimeSpan? timeout = null)
        {
            bool result = false;
            using (var db = this.GetContext())
            {
                var now = DateTime.Now;
                var m = db.SysTaskLock.Where(q => q.Type == type && q.Key == key).FirstOrDefault();
                if (m != null && (m.Status != LockStatus.Lock
                    || m.Owner == owner || m.ExpireTime < now))
                {
                    string updateSql = $"update SysTaskLock set {db.GetColumn("Status")} = {{0}},"
                        + $" {db.GetColumn("Owner")} = {{1}}, {db.GetColumn("ExpireTime")} = {{2}}, UpdateTime = {{3}}"
                        + " where Id = {4} and ("
                        + $"{db.GetColumn("Status")} <> {{5}} or {db.GetColumn("Owner")} = {{6}}"
                        + $" or {db.GetColumn("ExpireTime")} is not null and {db.GetColumn("ExpireTime")} < {{7}})";
                    object[] param = new object[8];
                    param[0] = LockStatus.Lock.GetValue();
                    param[1] = owner;
                    param[2] = null;
                    if (timeout.HasValue) param[2] = now.Add(timeout.Value);
                    param[3] = now;
                    param[4] = m.Id;
                    param[5] = LockStatus.Lock.GetValue();
                    param[6] = owner;
                    param[7] = now;

                    int count = db.ExecuteSqlCommand(updateSql, param);
                    result = count > 0;
                }
                else if (m == null)
                {
                    m = new SysTaskLock()
                    {
                        Id = IdGenerator.Get<SysTaskLock>(),
                        Type = type,
                        Key = key,
                        Status = LockStatus.Lock,
                        Owner = owner,
                        ExpireTime = null
                    };
                    if (timeout.HasValue) m.ExpireTime = now.Add(timeout.Value);
                    db.SysTaskLock.Add(m);
                    try
                    {
                        db.SaveChanges();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        Common.LogUtils.Warn("【DbLock】", ex);
                    }
                }
            }

            return result;
        }

        private bool RedisLock(TaskLockType type, string key, string owner, TimeSpan? timeout = null)
        {
            var islock = this.taskLockCache.Lock(type, key, timeout);
            if (islock)
            {
                this.taskLockOwnerCache.Set(type, key, owner, timeout);
            }
            else
            {
                var v = this.taskLockOwnerCache.Get(type, key);
                if (v == owner)//自己锁定
                {
                    islock = true;
                    this.taskLockCache.SetExpire(type, key, timeout);
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
        /// <param name="syncLockType">SyncLockType</param>
        /// <returns></returns>
        public virtual bool IsOtherLock(TaskLockType type, string key, string owner, SyncLockType syncLockType = SyncLockType.Database)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (key.Length > 50) throw new ArgumentOutOfRangeException(nameof(key), "Length不能大于50！");
            if (string.IsNullOrEmpty(owner)) throw new ArgumentNullException(nameof(owner));
            if (owner.Length > 50) throw new ArgumentOutOfRangeException(nameof(owner), "Length不能大于50！");
            if (ConfigUtils.CacheType != CacheType.Redis) throw new NotSupportedException($"系统缓存类型(ConfigUtils.CacheType={ConfigUtils.CacheType})不支持 SyncLockType={syncLockType}!");
            switch (syncLockType)
            {
                case SyncLockType.Database:
                    return this.DbIsOtherLock(type, key, owner);
                case SyncLockType.Redis:
                    return this.RedisIsOtherLock(type, key, owner);
                default:
                    throw new NotImplementedException($"SyncLockType: {syncLockType} 未实现！");
            }
        }

        private bool DbIsOtherLock(TaskLockType type, string key, string owner)
        {
            bool result = false;
            using (var db = this.GetContext())
            {
                var now = DateTime.Now;
                var m = db.SysTaskLock.Where(q => q.Type == type && q.Key == key).FirstOrDefault();
                if (m == null || m.Status != LockStatus.Lock
                    || m.ExpireTime.HasValue && m.ExpireTime < now
                    || m.Owner == owner)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        private bool RedisIsOtherLock(TaskLockType type, string key, string owner)
        {
            var islock = this.taskLockCache.IsLock(type, key);
            if (islock)
            {
                var v = this.taskLockOwnerCache.Get(type, key);
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
        /// <param name="syncLockType">SyncLockType</param>
        public virtual void Release(TaskLockType type, string key, SyncLockType syncLockType = SyncLockType.Database)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (key.Length > 50) throw new ArgumentOutOfRangeException(nameof(key), "Length不能大于50！");
            if (ConfigUtils.CacheType != CacheType.Redis) throw new NotSupportedException($"系统缓存类型(ConfigUtils.CacheType={ConfigUtils.CacheType})不支持 SyncLockType={syncLockType}!");
            switch (syncLockType)
            {
                case SyncLockType.Database:
                    this.DbRelease(type, key);
                    break;
                case SyncLockType.Redis:
                    this.RedisRelease(type, key);
                    break;
                default:
                    throw new NotImplementedException($"SyncLockType: {syncLockType} 未实现！");
            }
        }

        private void DbRelease(TaskLockType type, string key)
        {
            using (var db = this.GetContext())
            {
                var now = DateTime.Now;
                var m = db.SysTaskLock.Where(q => q.Type == type && q.Key == key).FirstOrDefault();
                if (m != null)
                {
                    m.Status = LockStatus.UnLock;
                    m.Owner = "*";
                    m.ExpireTime = now;
                    db.SaveChanges();
                }
            }
        }

        private void RedisRelease(TaskLockType type, string key)
        {
            this.taskLockCache.Release(type, key);
            this.taskLockOwnerCache.Remove(type, key);
        }

        /// <summary>
        /// 更新Timeout
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空</param>
        /// <param name="owner">锁定定者，不能为空</param>
        /// <param name="timeout">锁超时</param>
        /// <param name="syncLockType">SyncLockType</param>
        /// <returns></returns>
        public virtual void UpdateTimeout(TaskLockType type, string key, string owner, TimeSpan? timeout, SyncLockType syncLockType = SyncLockType.Database)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (key.Length > 50) throw new ArgumentOutOfRangeException(nameof(key), "Length不能大于50！");
            if (string.IsNullOrEmpty(owner)) throw new ArgumentNullException(nameof(owner));
            if (owner.Length > 50) throw new ArgumentOutOfRangeException(nameof(owner), "Length不能大于50！");
            if (ConfigUtils.CacheType != CacheType.Redis) throw new NotSupportedException($"系统缓存类型(ConfigUtils.CacheType={ConfigUtils.CacheType})不支持 SyncLockType={syncLockType}!");
            switch (syncLockType)
            {
                case SyncLockType.Database:
                    this.DbUpdateTimeout(type, key, owner, timeout);
                    break;
                case SyncLockType.Redis:
                    this.RedisUpdateTimeout(type, key, owner, timeout);
                    break;
                default:
                    throw new NotImplementedException($"SyncLockType: {syncLockType} 未实现！");
            }
        }

        private void DbUpdateTimeout(TaskLockType type, string key, string owner, TimeSpan? timeout)
        {
            using (var db = this.GetContext())
            {
                var now = DateTime.Now;
                var m = db.SysTaskLock.Where(q => q.Type == type && q.Key == key && q.Owner == owner).FirstOrDefault();
                if (m != null && m.Status == LockStatus.Lock)
                {
                    if (timeout.HasValue) m.ExpireTime = now.Add(timeout.Value);
                    else m.ExpireTime = null;
                    db.SaveChanges();
                }
            }
        }

        private void RedisUpdateTimeout(TaskLockType type, string key, string owner, TimeSpan? timeout)
        {
            this.taskLockCache.SetExpire(type, key, timeout);
            this.taskLockOwnerCache.SetExpire(type, key, timeout);
        }
    }
}
