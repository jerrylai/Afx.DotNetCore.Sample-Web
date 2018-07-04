using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.IRepository
{
    /// <summary>
    /// 系统相关存储接口
    /// </summary>
    public interface ISystemRepository : IBaseRepository
    {
        /// <summary>
        /// 更新数据库结构
        /// </summary>
        void InitDb();

        /// <summary>
        /// 初始化系统数据
        /// </summary>
        /// <returns></returns>
        void InitData();
        
    }
}
