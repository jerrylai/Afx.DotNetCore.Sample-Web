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
    public class Role : IModel, IIsDelete, IUpdateTime, ICreateTime
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

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
