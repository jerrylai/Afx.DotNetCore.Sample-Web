using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IRepository
{
    public interface IRegionRepository : IBaseRepository
    {
        RegionDto Get(string id);

        int Add(RegionDto vm);

        int Update(RegionDto vm);

        int Delete(string id);

        List<string> GetChild(string parentId);
    }
}
