using System;
using System.Collections.Generic;
using System.Text;

namespace AfxDotNetCoreSample.Dto
{
    public class TreeNodeDto
    {
        public string id { get; set; }

        public string text { get; set; }

        public string iconCls { get; set; }

        /// <summary>
        /// open, closed
        /// </summary>
        public string state { get; set; }

        public bool? @checked {get;set;}

        public string showText { get; set; }

        public object Obj { get; set; }

        public List<TreeNodeDto> children { get; set; }
    }
}
