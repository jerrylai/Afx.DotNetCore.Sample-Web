using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IService
{
    public interface IRoleWebMenuService : IBaseService
    {
        List<string> Get(string roleId);

        bool Exist(string roleId, string webMenuId);

        bool Update(string roleId, List<string> webMenuIdList);
    }
}
