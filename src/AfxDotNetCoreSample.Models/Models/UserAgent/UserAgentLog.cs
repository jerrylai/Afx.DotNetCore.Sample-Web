using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 用户代理人记录
    /// </summary>
    [Table("UserAgentLog")]
    public class UserAgentLog : IModel, ICreateTime
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column("Id")]
        [MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        ///用户 id
        /// </summary>
        [Required]
        [Column("UserId")]
        [MaxLength(50)]
        public string UserId { get; set; }

        [Column("AgentUserId")]
        [MaxLength(50)]
        public string AgentUserId { get; set; }

        [Column("AgentUserName")]
        [MaxLength(100)]
        public string AgentUserName { get; set; }

        [Required]
        [Column("IsEnabled")]
        public bool IsEnabled { get; set; }

        [Column("BeginTime")]
        public DateTime? BeginTime { get; set; }

        [Column("EndTime")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Column("CreateTime")]
        public DateTime CreateTime { get; set; }
    }
}
