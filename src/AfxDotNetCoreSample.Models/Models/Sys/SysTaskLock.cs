using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 执行状态锁
    /// </summary>
    [Table("SysTaskLock")]
    public class SysTaskLock : IModel, IUpdateTime, ICreateTime
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column("Id")]
        [MaxLength(50)]
        public string Id { get; set; }
        /// <summary>
        /// 枚举值（TaskLockType）
        /// </summary>
        [Required]
        [Column("Type")]
        [Index("IX_SysTaskLock", true)]
        public TaskLockType Type { get; set; }
        /// <summary>
        /// 任务key
        /// </summary>
        [Required]
        [Column("Key")]
        [MaxLength(50)]
        [Index("IX_SysTaskLock", true)]
        public string Key { get; set; }
        /// <summary>
        /// 使用者
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Column("Owner")]
        public string Owner { get; set; }
        /// <summary>
        /// 任务状态枚举（LockStatus）
        /// </summary>
        [Required]
        [Column("Status")]
        public LockStatus Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Column("ExecTime")]
        public DateTime ExecTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ExpireTime")]
        public DateTime? ExpireTime { get; set; }
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
