using System;

namespace AfxDotNetCoreSample.Enums
{
    /// <summary>
    /// 配置类型
    /// </summary>
    public enum UserStatus : int
    {
        None = 0,
        /// <summary>
        /// 启用
        /// </summary>
        Enabled = 1,
        /// <summary>
        /// 禁用
        /// </summary>
        Disabled = 2
    }
}
