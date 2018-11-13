using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.ICache
{
    public interface IRegionLevelCache : IBaseCache
    {
        List<RegionLevelDto> Get(string regionId);
        void Set(string regionId, List<RegionLevelDto> list);
        void Remove(string regionId);
    }
}
