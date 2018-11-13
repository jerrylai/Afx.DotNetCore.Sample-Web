using System;

using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Dto
{
    public interface ISyncLock: IDisposable
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="owner"></param>
        /// <param name="timeout"></param>
        void Init(LockType type, string key, string owner = null,
            TimeSpan? timeout = null);

        /// <summary>
        /// 任务类型 TaskLockType
        /// </summary>
        LockType Type { get; }
        /// <summary>
        /// 任务key
        /// </summary>
        string Key { get; }
        /// <summary>
        /// 锁定者
        /// </summary>
        string Owner { get; }
        /// <summary>
        /// 锁超时时间
        /// </summary>
        TimeSpan? Timeout { get; }
        
        /// <summary>
        /// 是否锁定，true：是，false：否
        /// </summary>
        bool IsLockSucceed { get; }

        /// <summary>
        /// 获取锁， true：获取成功，false：获取失败
        /// </summary>
        /// <returns></returns>
        bool Lock();
        /// <summary>
        /// 是否被其他锁定，true：是，false：否
        /// </summary>
        /// <returns></returns>
        bool IsOtherLock();
        /// <summary>
        /// 释放锁
        /// </summary>
        void Release();
        /// <summary>
        /// 更新锁Timeout
        /// </summary>
        void UpdateTimeout();
    }
}
