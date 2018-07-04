using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 用户代理人
    /// </summary>
    [Table("UserAgent")]
    public class UserAgent : IModel, ICreateTime, IUpdateTime
    {
        /// <summary>
        ///用户 id
        /// </summary>
        [Key]
        [Column("UserId")]
        [MaxLength(50)]
        public string UserId { get; set; }

        [Column("AgentUserId")]
        [MaxLength(50)]
        public string AgentUserId { get; set; }

        [Required]
        [Column("IsEnabled")]
        public bool IsEnabled { get; set; }

        [Column("BeginTime")]
        public DateTime? BeginTime { get; set; }

        [Column("EndTime")]
        public DateTime? EndTime { get; set; }

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
    }
}
