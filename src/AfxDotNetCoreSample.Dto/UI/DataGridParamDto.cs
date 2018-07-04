using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.Dto
{
    public class DataGridParamDto
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// 行数
        /// </summary>
        public int rows { get; set; }

        /// <summary>
        /// 排序列
        /// </summary>
        public string sort { get; set; }

        /// <summary>
        /// 'asc' or 'desc'
        /// </summary>
        public string order { get; set; }
    }
}
