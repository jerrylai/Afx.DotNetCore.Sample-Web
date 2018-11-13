using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace AfxDotNetCoreSample.Dto
{
    public class RegionDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 父级id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [Required,MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// 一级：1
        /// </summary>
        public int Level { get; set; }
    }
}
