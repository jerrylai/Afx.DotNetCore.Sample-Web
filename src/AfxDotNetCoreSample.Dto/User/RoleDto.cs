using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.Dto
{
    public class RoleDto
    {
        public string Id { get; set; }

        /// <summary>
        /// 角色类型枚举值
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 0.用户添加；1.系统内置
        /// </summary>
        public int IsSystem { get; set; }
    }
}
