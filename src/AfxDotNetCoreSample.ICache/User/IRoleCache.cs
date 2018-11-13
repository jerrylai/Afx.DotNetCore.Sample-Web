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
        RoleDto Get(string id);
        void Set(string id, RoleDto vm);
        void Remove(string id);
    }
}
