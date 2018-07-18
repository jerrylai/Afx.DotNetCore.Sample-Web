using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Models;

using Afx.Utils;

namespace AfxDotNetCoreSample.Repository
{
    /// <summary>
    /// 分布式任务锁实现
    /// </summary>
    public class TaskLockRepository : BaseRepository, ITaskLockRepository
    {

        /// <summary>
        /// 尝试获取锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key， 空字符或null需要换成*</param>
        /// <param name="owner">锁定定者， 空字符或null需要换成*</param>
        /// <param name="timeout">锁超时，单位秒</param>
        /// <returns></returns>
        public virtual bool Lock(TaskLockType type, string key, string owner, TimeSpan? timeout)
        {
            bool result = false;
            if (string.IsNullOrEmpty(key)) key = "*";
            if (string.IsNullOrEmpty(owner)) owner = "*";
            using (var db = this.GetContext())
            {
                var now = DateTime.Now;
                var m = db.SysTaskLock.Where(q => q.Type == type && q.Key == key).FirstOrDefault();
                if (m != null && (m.Status != LockStatus.Lock
                    || m.Owner == owner || m.ExpireTime < now))
                {
                    string updateSql = $"update SysTaskLock set {db.GetColumn("LockStatus")} = {{0}},"
                        + $" {db.GetColumn("Owner")} = {{1}}, {db.GetColumn("ExpireTime")} = {{2}}, UpdateTime = {{3}}"
                        + " where Id = {4} and ("
                        + $"{db.GetColumn("LockStatus")} <> {{5}} or {db.GetColumn("Owner")} = {{6}}"
                        + $" or {db.GetColumn("ExpireTime")} is not null and {db.GetColumn("ExpireTime")} < {{7}})";
                    object[] param = new object[8];
                    param[0] = LockStatus.Lock.GetValue();
                    param[1] = owner;
                    param[2] = timeout.HasValue ? (object)now.Add(timeout.Value) : null;
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
                        ExpireTime =null
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
                        Common.LogUtils.Warn("", ex);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 查询是否锁定
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key， 空字符或null需要换成*</param>
        /// <returns></returns>
        public virtual bool IsLock(TaskLockType type, string key)
        {
            bool result = false;
            if (string.IsNullOrEmpty(key)) key = "*";
            using (var db = this.GetContext())
            {
                var now = DateTime.Now;
                var m = db.SysTaskLock.Where(q => q.Type == type && q.Key == key).FirstOrDefault();
                if (m == null || m.Status != LockStatus.Lock
                    || m.ExpireTime.HasValue && m.ExpireTime < now)
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
        /// <param name="key">锁key， 空字符或null需要换成*</param>
        public virtual void Release(TaskLockType type, string key)
        {
            if (string.IsNullOrEmpty(key)) key = "*";
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
    }
}
