using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IRepository
{ 
    public interface IRegionLevelRepository : IBaseRepository
    {
        List<RegionLevelDto> GetList(string regionId);
    }
}
