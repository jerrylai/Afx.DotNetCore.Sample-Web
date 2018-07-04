using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfxDotNetCoreSample.Dto
{
    /// <summary>
    /// 任务锁
    /// </summary>
    public class TaskLockDto
    {
        /// <summary>
        /// 锁类型
        /// </summary>
        public int Type { get; set; }
        
        /// <summary>
        /// 锁定值
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// 锁定者
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// 锁定之后，释放超时，单位秒
        /// </summary>
        public long Timeout { get; set; }
    }
}
