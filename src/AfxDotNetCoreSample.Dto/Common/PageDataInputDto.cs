using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfxDotNetCoreSample.Dto
{
    public class PageDataInputDto
    {
        public int PageIndex { get; set; } 

        public int PageSize { get; set; }

        public string Orderby { get; set; }

        public bool SortDesc { get; set; }
    }
}
