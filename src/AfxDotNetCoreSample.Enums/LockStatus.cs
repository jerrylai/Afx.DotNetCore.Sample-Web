using System;

namespace AfxDotNetCoreSample.Enums
{
    /// <summary>
    /// 锁定状态
    /// </summary>
    public enum LockStatus : int
    {
        None = 0,
        /// <summary>
        /// 未锁定
        /// </summary>
        UnLock = 1,
        /// <summary>
        /// 锁定
        /// </summary>
        Lock = 2
    }

}
