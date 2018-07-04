using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
   public abstract  class TaskLockDbCache : BaseCache
    {
        public TaskLockDbCache() : base("TaskLockDb") { }
    }
}
