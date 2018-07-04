using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 操作日志
    /// </summary>
    [Table("OpLog")]
    public class OpLog : IModel, ICreateTime
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column("Id")]
        [MaxLength(50)]
        public string Id { get; set; }
        /// <summary>
        /// 系统模块枚举
        /// </summary>
        [Required]
        [Column("SysModule")]
        public int SysModule { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        [Required]
        [Column("Type")]
        public int Type { get; set; }
        /// <summary>
        /// 操作用户id
        /// </summary>
        [Required]
        [Column("UserId")]
        [MaxLength(50)]
        public string UserId { get; set; }
        /// <summary>
        /// 操作用户账号
        /// </summary>
        [Required]
        [Column("UserAccount")]
        [MaxLength(100)]
        public string UserAccount { get; set; }
        /// <summary>
        /// 操作用户姓名
        /// </summary>
        [Required]
        [Column("UserName")]
        [MaxLength(100)]
        public string UserName { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        [Required]
        [Column("RoleId")]
        [MaxLength(50)]
        public string RoleId { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        [Required]
        [Column("RoleName")]
        [MaxLength(100)]
        public string RoleName { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Column("DeptId")]
        public string DeptId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [MaxLength(200)]
        [Column("DeptName")]
        public string DeptName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Column("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        [Required]
        [Column("Content")]
        [MaxLength(2048)]
        public string Content { get; set; }
    }
}
