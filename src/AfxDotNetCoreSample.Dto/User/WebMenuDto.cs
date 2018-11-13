using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.Dto
{
    public class WebMenuDto
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// image url地址
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// url地址
        /// </summary>
        public string PageUrl { get; set; }

        public int Order { get; set; }

        /// <summary>
        /// 0.不是菜单；1.是菜单
        /// </summary>
        public bool IsMenu { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
    }
}
