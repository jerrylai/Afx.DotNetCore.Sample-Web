using System;

namespace AfxDotNetCoreSample.Enums
{
    /// <summary>
    /// 任务类型
    /// </summary>
    public enum TaskLockType : int
    {
        None = 0,
        /// <summary>
        /// 初始化系统数据
        /// </summary>
        InitSystemData = 1,
        AddUserAccount = 2,
    }

}
