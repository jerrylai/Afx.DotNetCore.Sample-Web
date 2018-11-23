using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Common;

namespace AfxDotNetCoreSample.Service
{
    public class RoleWebMenuService : BaseService, IRoleWebMenuService
    {
        protected virtual IRoleWebMenuRepository repository => this.GetRepository<IRoleWebMenuRepository>();

        protected virtual IRoleRepository roleRepository => this.GetRepository<IRoleRepository>();

        protected virtual IWebMenuRepository webMenuRepository => this.GetRepository<IWebMenuRepository>();

        public virtual List<string> Get(string roleId)
        {
            List<string> list = null;
            if(!string.IsNullOrEmpty(roleId))
            {
                list = repository.Get(roleId);
            }

            return list;
        }

        public virtual bool Exist(string roleId, string webMenuId)
        {
            var list = this.Get(roleId);

            return list.Contains(webMenuId);
        }

        public virtual bool Update(string roleId, List<string> webMenuIdList)
        {
            bool result = false;
            if (string.IsNullOrEmpty(roleId)) throw new ApiParamNullException(nameof(roleId));
            if (webMenuIdList == null) throw new ApiParamNullException(nameof(webMenuIdList));
            var role = this.roleRepository.Get(roleId);
            if (role == null) throw new ApiParamException(nameof(roleId));
            var oldlist = this.repository.Get(roleId);
            var alllist = this.webMenuRepository.GetList();
            var addlist = webMenuIdList.FindAll(q => !oldlist.Contains(q) && alllist.Exists(m=>m.Id == q));
            var dellist = oldlist.FindAll(q => !webMenuIdList.Contains(q));
            this.repository.Update(roleId, addlist, dellist);

            return result;
        }
    }
}
