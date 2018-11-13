using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User : IModel, IIsDelete, IUpdateTime, ICreateTime
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string RoleId { get; set; }
        
        /// <summary>
        /// 账号
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Index("IX_User_Account")]
        public string Account { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(255)]
        public string Password { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [MaxLength(50)]
        public string Mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(200)]
        public string Mail { get; set; }
        /// <summary>
        /// 状态枚举
        /// </summary>
        public UserStatus Status { get; set; }
        /// <summary>
        /// 0.用户添加；1.系统内置
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 0.未删除；1.已删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
