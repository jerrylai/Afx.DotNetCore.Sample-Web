using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class SysConfig : IModel, IUpdateTime, ICreateTime
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }
        /// <summary>
        /// 配置类型枚举（ConfigType）
        /// </summary>
        [Index("IX_SysConfig", true)]
        public ConfigType Type { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Index("IX_SysConfig", true)]
        public string Name { get; set; }
        /// <summary>
        /// 配置数据
        /// </summary>
        [Required]
        [MaxLength(2048)]
        public string Value { get; set; }
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
