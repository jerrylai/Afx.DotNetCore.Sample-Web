using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.ICache
{
    public interface IRoleCache : IBaseCache
    {
        List<RoleDto> Get();
        void Set(List<RoleDto> list);
        void Remove();
    }
}
