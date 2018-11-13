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
                var now = DateTime.Now;
                var m = db.SysDistributedLock.Where(q => q.Type == type && q.Key == key).FirstOrDefault();
                if (m != null && (m.Status != LockStatus.Lock
                    || m.Owner == owner || m.ExpireTime < now))
                {
                    string updateSql = $"update SysDistributedLock set {db.GetColumn("Status")} = {{0}},"
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
                    m = new SysDistributedLock()
                    {
                        Id = IdGenerator.Get<SysDistributedLock>(),
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
                var now = DateTime.Now;
                var m = db.SysDistributedLock.Where(q => q.Type == type && q.Key == key).FirstOrDefault();
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
                var now = DateTime.Now;
                var m = db.SysDistributedLock.Where(q => q.Type == type && q.Key == key).FirstOrDefault();
                if (m != null)
                {
                    m.Status = LockStatus.UnLock;
                    m.Owner = "*";
                    m.ExpireTime = now;
                    db.SaveChanges();
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
                var now = DateTime.Now;
                var m = db.SysDistributedLock.Where(q => q.Type == type && q.Key == key && q.Owner == owner).FirstOrDefault();
                if (m != null && m.Status == LockStatus.Lock)
                {
                    if (timeout.HasValue) m.ExpireTime = now.Add(timeout.Value);
                    else m.ExpireTime = null;
                    db.SaveChanges();
                }
            }
        }
    }
}
