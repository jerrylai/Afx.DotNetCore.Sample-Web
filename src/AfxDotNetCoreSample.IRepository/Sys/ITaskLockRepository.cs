using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.IRepository
{
    /// <summary>
    /// 分布式任务锁接口
    /// </summary>
    public interface ITaskLockRepository : IBaseRepository
    {
        /// <summary>
        /// 尝试获取锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空, key长度小于或等于50</param>
        /// <param name="owner">锁定定者，不能为空, owner长度小于或等于50</param>
        /// <param name="timeout">锁超时</param>
        /// <param name="syncLockType">SyncLockType</param>
        /// <returns></returns>
        bool Lock(TaskLockType type, string key, string owner, TimeSpan? timeout = null, SyncLockType syncLockType = SyncLockType.Database);

        /// <summary>
        /// 查询是否锁定
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空，key长度小于或等于50</param>
        /// <param name="owner">锁定定者，不能为空, owner长度小于或等于50</param>
        /// <param name="syncLockType">SyncLockType</param>
        /// <returns></returns>
        bool IsOtherLock(TaskLockType type, string key, string owner, SyncLockType syncLockType = SyncLockType.Database);

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空，key长度小于或等于50</param>
        /// <param name="syncLockType">SyncLockType</param>
        void Release(TaskLockType type, string key, SyncLockType syncLockType = SyncLockType.Database);

        /// <summary>
        /// 更新Timeout
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空，key长度小于或等于50</param>
        /// <param name="owner">锁定定者，不能为空, owner长度小于或等于50</param>
        /// <param name="timeout">锁超时</param>
        /// <param name="syncLockType">SyncLockType</param>
        /// <returns></returns>
        void UpdateTimeout(TaskLockType type, string key, string owner, TimeSpan? timeout, SyncLockType syncLockType = SyncLockType.Database);
    }
}
