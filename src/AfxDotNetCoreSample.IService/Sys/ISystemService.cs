using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.IService
{
    public interface ISystemService : IBaseService
    {
        /// <summary>
        /// 初始化系统
        /// </summary>
        /// <returns></returns>
        void Init();
    }
}
