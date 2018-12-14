using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.IService
{
    /// <summary>
    /// 分布式任务锁业务接口
    /// </summary>
    public interface IDistributedLockService : IBaseService
    {
        /// <summary>
        /// 尝试获取锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空, key长度小于或等于50</param>
        /// <param name="owner">锁定定者，不能为空, owner长度小于或等于50</param>
        /// <param name="timeout">锁超时</param>
        /// <returns></returns>
        bool Lock(LockType type, string key, string owner, TimeSpan? timeout);

        /// <summary>
        /// 查询是否锁定
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空，key长度小于或等于50</param>
        /// <param name="owner">锁定定者，不能为空, owner长度小于或等于50</param>
        /// <returns></returns>
        bool IsOtherLock(LockType type, string key, string owner);

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空，key长度小于或等于50</param>
        void Release(LockType type, string key);

        /// <summary>
        /// 更新Timeout
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空，key长度小于或等于50</param>
        /// <param name="owner">锁定定者，不能为空, owner长度小于或等于50</param>
        /// <param name="timeout">锁超时</param>
        /// <returns></returns>
        void UpdateTimeout(LockType type, string key, string owner, TimeSpan? timeout);
    }
}
