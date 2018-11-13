using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.IService;

namespace AfxDotNetCoreSample.Web.Controllers
{
    public class RoleController : BaseController
    {
        private readonly Lazy<IRoleService> roleService = new Lazy<IRoleService>(IocUtils.Get<IRoleService>);

        [UserAuth("1001001000000")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [UserAuth]
        public IActionResult GetPageData(RolePageInputDto vm)
        {
            if (vm != null && this.ModelState.IsValid)
            {
                var data = this.roleService.Value.GetPage(vm);

                return Success(data);
            }

            return Error();
        }

        [HttpPost]
        [UserAuth("1001001000000")]
        public IActionResult Edit(RoleDto vm)
        {
            if (vm != null && this.ModelState.IsValid)
            {
                var userinfo = this.UserSession;
                if (!string.IsNullOrEmpty(vm.Id))
                {
                    var m = this.roleService.Value.Get(vm.Id);
                    if (m != null)
                    {
                        this.roleService.Value.Update(vm);
                        LogUtils.Debug($"【修改角色】{userinfo.Name}({userinfo.Account})，修改：{m.Name} -> {vm.Name}！");
                    }
                }
                else
                {
                    this.roleService.Value.Add(vm);
                    LogUtils.Debug($"【添加角色】{userinfo.Name}({userinfo.Account})，添加 {vm.Name}！");
                }

                return Success(true);
            }

            return Error();
        }

        [UserAuth("1001001000000")]
        public IActionResult Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var m = this.roleService.Value.Get(id);
                if (m != null)
                {
                    var result = this.roleService.Value.Delete(id);
                    var userinfo = this.UserSession;
                    LogUtils.Debug($"【删除角色】{userinfo.Name}({userinfo.Account})，删除 {m.Name}！");

                    return Success(result);
                }
            }
            return Error();
        }

        [UserAuth]
        public IActionResult Get(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                var m = this.roleService.Value.Get(id);

                return Success(m);
            }

            return Error();
        }
    }
}