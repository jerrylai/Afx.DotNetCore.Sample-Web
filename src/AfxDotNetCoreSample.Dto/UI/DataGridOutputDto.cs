using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.Dto
{
    public class DataGridOutputDto<T>
    {
        public int total { get; set; }

        public List<T> rows { get; set; }
    }
}
