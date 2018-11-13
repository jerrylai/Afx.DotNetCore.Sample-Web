using System;
using System.Collections.Generic;
using System.Text;

namespace AfxDotNetCoreSample.Dto
{
    public class RegionLevelDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Region id
        /// </summary>
        public string RegionId { get; set; }
        /// <summary>
        /// 父级id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 父级层级,一级：1
        /// </summary>
        public int ParentLevel { get; set; }
    }
}
