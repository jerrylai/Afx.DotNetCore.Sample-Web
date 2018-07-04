using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 角色
    /// </summary>
    [Table("Role")]
    public class Role : IModel, IIsDelete, IUpdateTime, ICreateTime
    {
        [Key]
        [Column("Id")]
        [MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        /// 角色类型枚举值
        /// </summary>
        [Required]
        [Column("Type")]
        public RoleType Type { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [Column("Name")]
        [MaxLength(200)]
        public string Name { get; set; }

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
