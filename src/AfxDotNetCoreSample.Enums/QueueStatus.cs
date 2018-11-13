using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.Enums
{
    public enum QueueStatus : int
    {
        /// <summary>
        /// 未知
        /// </summary>
        None = 0,
        /// <summary>
        /// 开始
        /// </summary>
        Begin = 1,
        /// <summary>
        /// 执行中
        /// </summary>
        Running = 2,
        /// <summary>
        /// 完成
        /// </summary>
        Finish = 3
    }

}
