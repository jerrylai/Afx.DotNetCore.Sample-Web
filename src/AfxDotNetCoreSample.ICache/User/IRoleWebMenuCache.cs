using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.ICache
{
    public interface IRoleWebMenuCache : IBaseCache
    {
        List<string> Get(string roleId);
        void Set(string roleId, List<string> webMenuIdList);
        void Remove(string roleId);
    }
}
