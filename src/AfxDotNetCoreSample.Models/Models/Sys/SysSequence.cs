using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 序列
    /// </summary>
    [Table("SysSequence")]
    public class SysSequence : IModel, ICreateTime, IUpdateTime
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column("Id")]
        [MaxLength(50)]
        public string Id { get; set; }
        /// <summary>
        /// 序列名称
        /// </summary>
        [Required]
        [Column("Name")]
        [MaxLength(100)]
        [Index("IX_SysSequence", true)]
        public string Name { get; set; }
        /// <summary>
        /// 序列key
        /// </summary>
        [Required]
        [Column("Key")]
        [MaxLength(100)]
        [Index("IX_SysSequence", true)]
        public string Key { get; set; }
        /// <summary>
        /// 当前值
        /// </summary>
        [Required]
        [Column("Value")]
        public int Value { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Column("UpdateTime")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Required]
        [Column("CreateTime")]
        public DateTime CreateTime { get; set; }
    }
}
