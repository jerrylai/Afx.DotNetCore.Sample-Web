using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class RoleCache : ListDbCache, IRoleCache
    {
        public virtual List<RoleDto> Get()
        {
            return base.GetData<List<RoleDto>>();
        }

        public virtual void Remove()
        {
            base.RemoveKey();
        }

        public virtual void Set(List<RoleDto> list)
        {
            base.SetData(list);
        }
    }
}
