﻿using System;
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
        private readonly Lazy<IRoleWebMenuRepository> _repository = new Lazy<IRoleWebMenuRepository>(() => IocUtils.Get<IRoleWebMenuRepository>());
        internal protected virtual IRoleWebMenuRepository repository => this._repository.Value;

        private Lazy<IRoleRepository> _roleRepository = new Lazy<IRoleRepository>(IocUtils.Get<IRoleRepository>);
        private IRoleRepository roleRepository => this._roleRepository.Value;

        private readonly Lazy<IWebMenuRepository> _webMenuRepository = new Lazy<IWebMenuRepository>(() => IocUtils.Get<IWebMenuRepository>());
        internal protected virtual IWebMenuRepository webMenuRepository => this._webMenuRepository.Value;

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
