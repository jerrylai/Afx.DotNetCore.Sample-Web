using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 分布式锁
    /// </summary>
    public class SysDistributedLock : IModel, IUpdateTime, ICreateTime
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }
        /// <summary>
        /// 枚举值（LockType）
        /// </summary>
        [Index("IX_SysDistributedLock", true)]
        public LockType Type { get; set; }
        /// <summary>
        /// 任务key
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Index("IX_SysDistributedLock", true)]
        public string Key { get; set; }
        /// <summary>
        /// 使用者
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Owner { get; set; }
        /// <summary>
        /// 任务状态枚举（LockStatus）
        /// </summary>
        public LockStatus Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ExpireTime { get; set; }
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
