using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.IService
{
    public interface IRegionService : IBaseService
    {
        RegionDto Get(string id);

        bool Add(RegionDto vm);

        bool Update(RegionDto vm);

        bool Delete(string id);

        List<string> GetChildId(string parentId);

        List<RegionDto> GetChildList(string parentId);

        List<RegionDto> GetParentList(string id);

        List<RegionLevelDto> GetLevelList(string id);

        string GetFullName(string id, string separator = null);
    }
}
