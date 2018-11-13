using System;
using System.Collections.Generic;
using System.Text;

namespace AfxDotNetCoreSample.Dto.Sys
{
    public class OpLogDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 系统模块枚举
        /// </summary>
        public int SysModule { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 操作用户id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 操作用户账号
        /// </summary>
        public string UserAccount { get; set; }
        /// <summary>
        /// 操作用户姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        public string Content { get; set; }
    }
}
