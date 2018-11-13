using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class OpLog : IModel, ICreateTime
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 角色id
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string RoleId { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string RoleName { get; set; }

        /// <summary>
        /// 操作用户id
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserId { get; set; }
        /// <summary>
        /// 操作用户账号
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string UserAccount { get; set; }
        /// <summary>
        /// 操作用户姓名
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }
        
        /// <summary>
        /// 操作内容
        /// </summary>
        [Required]
        [MaxLength(512)]
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
