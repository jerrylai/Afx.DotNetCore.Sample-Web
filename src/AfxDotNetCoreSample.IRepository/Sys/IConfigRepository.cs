using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.IRepository
{
    /// <summary>
    /// 系统配置接口
    /// </summary>
    public interface IConfigRepository : IBaseRepository
    {
        /// <summary>
        /// 根据配置类型获取
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <returns></returns>
        Dictionary<string, string> Get(ConfigType type);

        /// <summary>
        /// 获取单个系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <returns></returns>
        string GetValue(ConfigType type);

        /// <summary>
        /// 获取单个系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <param name="name">名称, 空字符串或null需要改成*</param>
        /// <returns></returns>
        string GetValue(ConfigType type, string name);

        /// <summary>
        /// 更新系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <param name="value"></param>
        /// <returns></returns>
        int SetValue(ConfigType type, string value);

        /// <summary>
        /// 更新系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <param name="name">名称, 空字符串或null需要改成*</param>
        /// <param name="value"></param>
        /// <returns></returns>
        int SetValue(ConfigType type, string name, string value);

        /// <summary>
        /// 更新系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <param name="dic">系统配置集合</param>
        /// <returns></returns>
        int SetValue(ConfigType type, Dictionary<string, string> dic);
    }
}
