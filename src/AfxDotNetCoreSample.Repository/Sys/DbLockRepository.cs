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
using System.Data;

namespace AfxDotNetCoreSample.Repository
{
    /// <summary>
    /// 分布式任务锁实现
    /// </summary>
    public class DbLockRepository : BaseRepository, ITaskLockRepository
    {
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
            bool result = false;
            using (var db = this.GetContext())
            {
                string id = null;
                using (db.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    id = db.SysDistributedLock.Where(q => q.Type == type && q.Key == key).Select(q => q.Id).FirstOrDefault();
                    db.Commit();
                }

                var now = DateTime.Now;
                SysDistributedLock m = null;
                if (!string.IsNullOrEmpty(id)) m = db.SysDistributedLock.Where(q => q.Id == id).FirstOrDefault();
                if (m != null && (m.Status != LockStatus.Lock
                    || m.Owner == owner || m.ExpireTime < now))
                {
                    var state = db.Entry(m);
                    m.Status = LockStatus.Lock;
                    state.Property<LockStatus>(nameof(m.Status)).IsModified = true;
                    m.Owner = owner;
                    state.Property<string>(nameof(m.Owner)).IsModified = true;
                    if (timeout.HasValue) m.ExpireTime = now.Add(timeout.Value);
                    else m.ExpireTime = null;
                    state.Property<DateTime?>(nameof(m.ExpireTime)).IsModified = true;

                    int count = db.SaveChanges();

                    result = count > 0;
                }
                else if (m == null)
                {
                    m = new SysDistributedLock()
                    {
                        Id = this.GetIdentity<SysDistributedLock>(),
                        Type = type,
                        Key = key,
                        Status = LockStatus.Lock,
                        Owner = owner,
                        ExpireTime = null
                    };
                    if (timeout.HasValue) m.ExpireTime = now.Add(timeout.Value);
                    db.SysDistributedLock.Add(m);
                    try
                    {
                        int count = db.SaveChanges();
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

        /// <summary>
        /// 查询是否锁定
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key不能为空</param>
        /// <param name="owner">锁定定者，不能为空</param>
        /// <returns></returns>
        public virtual bool IsOtherLock(LockType type, string key, string owner)
        {
            bool result = false;
            using (var db = this.GetContext())
            {
                string id = null;
                using (db.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    id = db.SysDistributedLock.Where(q => q.Type == type && q.Key == key).Select(q=>q.Id).FirstOrDefault();
                    db.Commit();
                }
                if (!string.IsNullOrEmpty(id))
                {
                    var m = db.SysDistributedLock.Where(q => q.Id == id).FirstOrDefault();
                    if(m != null && m.Status == LockStatus.Lock && m.Owner != owner
                        && (!m.ExpireTime.HasValue || m.ExpireTime > DateTime.Now))
                    {
                        result = true;
                    }
                }
            }

            return result;
        }
        
        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空</param>
        /// <param name="syncLockType">SyncLockType</param>
        public virtual void Release(LockType type, string key)
        {
            using (var db = this.GetContext())
            {
                string id = null;
                using (db.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    id = db.SysDistributedLock.Where(q => q.Type == type && q.Key == key).Select(q => q.Id).FirstOrDefault();
                    db.Commit();
                }
                if(!string.IsNullOrEmpty(id))
                {
                    var now = DateTime.Now;
                    var m = db.SysDistributedLock.Where(q => q.Id == id).FirstOrDefault();
                    if (m != null)
                    {
                        m.Status = LockStatus.UnLock;
                        m.Owner = "*";
                        m.ExpireTime = now;
                        db.SaveChanges();
                    }
                }
            }
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
        public virtual void UpdateTimeout(LockType type, string key, string owner, TimeSpan? timeout)
        {
            using (var db = this.GetContext())
            {
                string id = null;
                using (db.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    id = db.SysDistributedLock.Where(q => q.Type == type && q.Key == key).Select(q => q.Id).FirstOrDefault();
                    db.Commit();
                }
                if (!string.IsNullOrEmpty(id))
                {
                    var now = DateTime.Now;
                    var m = db.SysDistributedLock.Where(q => q.Id == id).FirstOrDefault();
                    if (m != null && m.Owner == owner && m.Status == LockStatus.Lock)
                    {
                        if (timeout.HasValue) m.ExpireTime = now.Add(timeout.Value);
                        else m.ExpireTime = null;
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
