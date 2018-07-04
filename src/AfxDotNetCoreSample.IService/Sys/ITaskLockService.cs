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
    public interface ITaskLockService : IBaseService
    {
        /// <summary>
        /// 尝试获取锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key， 空字符或null需要换成*</param>
        /// <param name="owner">锁定定者， 空字符或null需要换成*</param>
        /// <param name="timeout">锁定之后，释放超时，单位秒</param>
        /// <returns></returns>
        bool Lock(TaskLockType type, string key, string owner, TimeSpan? timeout);

        /// <summary>
        /// 查询是否锁定
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key， 空字符或null需要换成*</param>
        /// <returns></returns>
        bool IsLock(TaskLockType type, string key);

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key， 空字符或null需要换成*</param>
        void Release(TaskLockType type, string key);
    }
}
