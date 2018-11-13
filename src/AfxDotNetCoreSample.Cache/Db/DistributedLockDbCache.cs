using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
   public abstract  class DistributedLockDbCache<TCache> : BaseCache<TCache>
    {
        public DistributedLockDbCache() : base("DistributedLockDb") { }
    }
}
