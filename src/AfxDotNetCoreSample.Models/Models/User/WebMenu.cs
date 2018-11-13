using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// WEB菜单
    /// </summary>
    public class WebMenu : IModel
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }

        [MaxLength(50)]
        public string ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// image url地址
        /// </summary>
        [MaxLength(200)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// url地址
        /// </summary>
        [MaxLength(200)]
        public string PageUrl { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 0.不是菜单；1.是菜单
        /// </summary>
        public bool IsMenu { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [MaxLength(255)]
        public string Description { get; set; }
    }
}
