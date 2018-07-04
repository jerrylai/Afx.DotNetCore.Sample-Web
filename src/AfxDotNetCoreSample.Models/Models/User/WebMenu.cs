using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// WEB菜单
    /// </summary>
    [Table("WebMenu")]
    public class WebMenu : IModel
    {
        [Key]
        [Column("Id")]
        [MaxLength(50)]
        public string Id { get; set; }

        [Column("ParentId")]
        [MaxLength(50)]
        public string ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [Column("Name")]
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// url地址
        /// </summary>
        [Column("Url")]
        [MaxLength(200)]
        public string Url { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Required]
        [Column("Order")]
        public int Order { get; set; }

        /// <summary>
        /// 0.不是菜单；1.是菜单
        /// </summary>
        [Required]
        [Column("IsMenu")]
        public bool IsMenu { get; set; }
    }
}
