using AfxDotNetCoreSample.Enums;
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
    public class DistributedLockDto
    {
        /// <summary>
        /// 锁类型
        /// </summary>
        public LockType Type { get; set; }
        
        /// <summary>
        /// 锁定值, length 小于等于 50
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 锁定者, length 小于等于 50
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// 锁定之后，超时释放时间
        /// </summary>
        public TimeSpan? Timeout { get; set; }
    }
}
