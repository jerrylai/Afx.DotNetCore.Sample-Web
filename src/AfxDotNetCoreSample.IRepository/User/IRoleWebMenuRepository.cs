using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.IRepository
{
    public interface IRoleWebMenuRepository : IBaseRepository
    {
        List<string> Get(string roleId);

        int Update(string roleId, List<string> addWebMenuIdList, List<string> delWebMenuIdList);
    }
}
