using System;

namespace AfxDotNetCoreSample.Enums
{
    /// <summary>
    /// 分布式锁类型
    /// </summary>
    public enum LockType : int
    {
        None = 0,
        /// <summary>
        /// 初始化系统数据
        /// </summary>
        InitSystemData = 1,
        /// <summary>
        /// 添加用户账号锁
        /// </summary>
        AddUserAccount = 2,
        /// <summary>
        /// 队列锁
        /// </summary>
        SysQueue = 3,
        AddUserMobile = 4,
        AddUserMail = 5
    }

}
