using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class RegionCache : DataDbCache<RegionCache>, IRegionCache
    {
        public virtual RegionDto Get(string id)
        {
            return this.GetData<RegionDto>(id);
        }

        public virtual void Remove(string id)
        {
            this.RemoveKey(id);
        }

        public virtual void Set(string id, RegionDto m)
        {
            this.SetData(m, id);
        }
    }
}
