using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class RoleWebMenuCache : DataDbCache, IRoleWebMenuCache
    {
        public virtual List<string> Get(string roleId)
        {
            return base.Get<List<string>>(roleId);
        }

        public virtual void Remove(string roleId)
        {
            base.Remove(roleId);
        }

        public virtual void Set(string roleId, List<string> webMenuIdList)
        {
            base.Set(webMenuIdList, roleId);
        }
    }
}
