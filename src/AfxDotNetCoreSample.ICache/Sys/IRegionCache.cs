using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.ICache
{
    public interface IRegionCache : IBaseCache
    {
        RegionDto Get(string id);
        void Set(string id, RegionDto m);
        void Remove(string id);
    }
}
