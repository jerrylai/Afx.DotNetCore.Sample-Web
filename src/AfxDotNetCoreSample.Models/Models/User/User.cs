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
    [Table("User")]
    public class User : IModel, IIsDelete, IUpdateTime, ICreateTime
    {
        [Key]
        [Column("Id")]
        [MaxLength(50)]
        public string Id { get; set; }
        /// <summary>
        /// 角色Id
        /// </summary>
        [Required]
        [Column("RoleId")]
        [MaxLength(50)]
        public string RoleId { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        [Required]
        [Column("Account")]
        [MaxLength(100)]
        public string Account { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [Column("Name")]
        [MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Column("Password")]
        [MaxLength(255)]
        public string Password { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [MaxLength(50)]
        [Column("Mobile")]
        public string Mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [Column("Mail")]
        [MaxLength(200)]
        public string Mail { get; set; }
        /// <summary>
        /// 状态枚举
        /// </summary>
        [Required]
        [Column("Status")]
        public UserStatus Status { get; set; }
        /// <summary>
        /// 0.用户添加；1.系统内置
        /// </summary>
        [Required]
        [Column("IsSystem")]
        public bool IsSystem { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Required]
        [Column("UpdateTime")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Column("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 0.未删除；1.已删除
        /// </summary>
        [Required]
        [Column("IsDelete")]
        public bool IsDelete { get; set; }
    }
}
