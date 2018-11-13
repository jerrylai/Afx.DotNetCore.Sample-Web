using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfxDotNetCoreSample.Models
{
    public class RegionLevel : IModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }
        /// <summary>
        /// Region id
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string RegionId { get; set; }
        /// <summary>
        /// 父级id
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ParentId { get; set; }
        /// <summary>
        /// 父级层级,一级：1
        /// </summary>
        public int ParentLevel { get; set; }
    }
}
