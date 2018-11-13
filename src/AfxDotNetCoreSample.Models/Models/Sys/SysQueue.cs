using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 系统队列表
    /// </summary>
    public class SysQueue : IModel, IUpdateTime, ICreateTime
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [MaxLength(50)]
        public string Id { set; get; }
        /// <summary>
        /// 枚举值（QueueType）
        /// </summary>
        public QueueType Type { set; get; }
        /// <summary>
        /// 任务数据
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Value { set; get; }
        /// <summary>
        /// 任务状态枚举（QueueStatus）
        /// </summary>
        public QueueStatus Status { set; get; }
        
        /// <summary>
        /// 任务开始时间
        /// </summary>
        public DateTime ExecTime { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { set; get; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { set; get; }
        
    }
}
