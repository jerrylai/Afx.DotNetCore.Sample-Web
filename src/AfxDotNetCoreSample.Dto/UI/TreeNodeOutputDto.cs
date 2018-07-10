using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.Dto
{
    public class TreeNodeOutputDto
    {
        public string id { get; set; }

        public string text { get; set; }

        public string url { get; set; }

        /// <summary>
        /// closed
        /// </summary>
        public string state { get; set; }

        public string iconCls { get; set; }

        public List<TreeNodeOutputDto> children { get; set; }
    }
}
