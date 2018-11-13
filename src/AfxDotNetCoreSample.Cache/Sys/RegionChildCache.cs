using AfxDotNetCoreSample.ICache;
using System;
using System.Collections.Generic;
using System.Text;

namespace AfxDotNetCoreSample.Cache
{
    public class RegionChildCache : DataDbCache<RegionChildCache>, IRegionChildCache
    {
        public virtual List<string> Get(string parentId)
        {
           return this.GetData<List<string>>(parentId);
        }

        public virtual void Remove(string parentId)
        {
            this.RemoveKey(parentId);
        }

        public virtual void Set(string parentId, List<string> list)
        {
            this.SetData(list, parentId);
        }
    }
}
