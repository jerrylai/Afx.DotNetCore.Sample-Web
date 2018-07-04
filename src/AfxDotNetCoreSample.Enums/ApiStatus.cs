using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.Enums
{
    public enum ApiStatus : int
    {
        /// <summary>
        /// 成功！
        /// </summary>
        Success = 0,
        /// <summary>
        /// 失败！
        /// </summary>
        Failure = 100,
        /// <summary>
        /// 参数错误
        /// </summary>
        ParamError = 101,
        /// <summary>
        /// 服务器错误！
        /// </summary>
        ServerError = 102,
        /// <summary>
        /// 未登录或登录已超时！
        /// </summary>
        NeedLogin = 200,
        /// <summary>
        /// 未授权！
        /// </summary>
        NeedAuth = 201,
        /// <summary>
        /// 需要授权！
        /// </summary>
        NeedLicense = 300
    }
}
