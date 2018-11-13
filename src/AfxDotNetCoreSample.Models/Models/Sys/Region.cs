using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 区域
    /// </summary>
    public class Region : IModel, ICreateTime, IUpdateTime, IIsDelete
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }
        /// <summary>
        /// 父级id
        /// </summary>
        [MaxLength(50)]
        public string ParentId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        /// <summary>
        /// 一级：1
        /// </summary>
        public int Level { get; set; }

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
