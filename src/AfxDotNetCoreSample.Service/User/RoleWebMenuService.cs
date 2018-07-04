using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.IRepository;

namespace AfxDotNetCoreSample.Service
{
    public class RoleWebMenuService : BaseService, IRoleWebMenuService
    {
        public virtual List<string> Get(string roleId)
        {
            List<string> list = null;
            if(!string.IsNullOrEmpty(roleId))
            {
                var repository = this.GetRepository<IRoleWebMenuRepository>();
                list = repository.Get(roleId);
            }

            return list;
        }

        public virtual bool Exist(string roleId, string webMenuId)
        {
            var list = this.Get(roleId);

            return list.Contains(webMenuId);
        }
    }
}
