using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;
using System;
using System.Collections.Generic;
using System.Text;

namespace AfxDotNetCoreSample.Cache
{
    public class RegionLevelCache : DataDbCache<RegionLevelCache>, IRegionLevelCache
    {
        public virtual List<RegionLevelDto> Get(string regionId)
        {
            return this.GetData<List<RegionLevelDto>>(regionId);
        }

        public virtual void Remove(string regionId)
        {
            this.RemoveKey(regionId);
        }

        public virtual void Set(string regionId, List<RegionLevelDto> list)
        {
            this.SetData(list, regionId);
        }
    }
}
