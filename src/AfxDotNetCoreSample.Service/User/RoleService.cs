using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.Common;
using Afx.Utils;

namespace AfxDotNetCoreSample.Service
{
    public class RoleService : BaseService, IRoleService
    {
        protected virtual IRoleRepository roleRepository => this.GetRepository<IRoleRepository>();

        protected virtual IUserRepository userRepository => this.GetRepository<IUserRepository>();

        public virtual bool Add(RoleDto vm)
        {
            if (vm == null) throw new ApiException(ApiStatus.Error, "vm不能为空!");
            vm.IsSystem = false;
            int count = this.roleRepository.Add(vm);

            return count > 0;
        }

        public virtual bool Delete(string id)
        {
            var vm = this.roleRepository.Get(id);
            if (vm == null) throw new ApiException("角色不存在！");
            if (vm.IsSystem == true) throw new ApiException("系统默认角色不能删除！");
            var usercount = this.userRepository.GetUserCount(id);
            if(usercount > 0) throw new ApiException("角色已使用不能删除！");
            int count = this.roleRepository.Delete(id);

            return count > 0;
        }

        public virtual RoleDto Get(string id)
        {
            var vm = this.roleRepository.Get(id);

            return vm;
        }

        public virtual PageDataOutputDto<RoleDto> GetPage(RolePageInputDto vm)
        {
            if (vm == null) throw new ApiParamNullException(nameof(vm));
            if (vm.PageIndex < 1) throw new ApiParamException(nameof(vm.PageIndex));
            if (vm.PageSize < 1) throw new ApiParamException(nameof(vm.PageSize));
            var pagedata = this.roleRepository.GetPage(vm);

            return pagedata;
        }

        public virtual bool Update(RoleDto vm)
        {
            if (vm == null) throw new ApiException(ApiStatus.Error, "vm不能为空!");
            if (string.IsNullOrEmpty(vm.Id)) throw new ApiException(ApiStatus.Error, "vm.Id不能为空!");
            var m = this.roleRepository.Get(vm.Id);
            if (m == null) throw new ApiException("角色不存在！");
            //if (m.IsSystem == true) throw new ApiException("系统默认角色不能修改！");
            var count = this.roleRepository.Update(vm);

            return true;
        }
    }
}
