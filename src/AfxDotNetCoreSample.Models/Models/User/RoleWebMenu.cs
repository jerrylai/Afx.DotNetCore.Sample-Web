using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 角色WEB菜单关系
    /// </summary>
    [Table("RoleWebMenu")]
    public class RoleWebMenu : IModel
    {
        [Key]
        [Column("Id")]
        [MaxLength(50)]
        public string Id { get; set; }

        [Required]
        [Column("RoleId")]
        [MaxLength(50)]
        public string RoleId { get; set; }

        [Required]
        [Column("WebMenuId")]
        [MaxLength(50)]
        public string WebMenuId { get; set; }
    }
}
