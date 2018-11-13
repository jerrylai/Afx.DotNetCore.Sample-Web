using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 序列
    /// </summary>
    public class SysSequence : IModel, ICreateTime, IUpdateTime
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }
        /// <summary>
        /// 序列名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Index("IX_SysSequence", true)]
        public string Name { get; set; }
        /// <summary>
        /// 序列key
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Index("IX_SysSequence", true)]
        public string Key { get; set; }
        /// <summary>
        /// 当前值
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
