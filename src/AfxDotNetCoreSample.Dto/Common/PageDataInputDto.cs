using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Dto
{
    public class PageDataInputDto
    {
        public string Keyword { get; set; }

        [IntValue(0)]
        public int PageIndex { get; set; }

        [IntValue(0)]
        public int PageSize { get; set; }

        /// <summary>
        /// Name desc, Id asc
        /// </summary>
        public string Orderby { get; set; }
    }
}
