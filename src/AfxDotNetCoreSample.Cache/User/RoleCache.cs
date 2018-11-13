using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class RoleCache : DataDbCache<RoleCache>, IRoleCache
    {
        public virtual RoleDto Get(string id)
        {
            return base.GetData<RoleDto>(id);
        }

        public virtual void Remove(string id)
        {
            base.RemoveKey(id);
        }

        public virtual void Set(string id, RoleDto vm)
        {
            base.SetData(vm, id);
        }
    }
}
