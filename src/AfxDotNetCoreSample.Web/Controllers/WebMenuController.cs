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
    public class WebMenuController : BaseController
    {
        private IWebMenuService webMenuService => this.GetService<IWebMenuService>();
        private IRoleWebMenuService roleWebMenuService => this.GetService<IRoleWebMenuService>();

        private IRoleService roleService => this.GetService<IRoleService>();

        [UserAuth]
        public IActionResult GetUserMenu()
        {
           var list = webMenuService.GetList();
            var ids = this.roleWebMenuService.Get(this.UserSession.RoleId);
            list = list.FindAll(q => ids.Contains(q.Id));

            var treelist = this.GetTreeList(null, list, true, false);

            return Success(treelist);
        }

        [UserAuth]
        public IActionResult GetAll()
        {
            var list = webMenuService.GetList();
            var treelist = this.GetTreeList(null, list, null, true);

            return Success(treelist);
        }

        private List<TreeNodeDto> GetTreeList(string parentId, List<WebMenuDto> list, bool? isMenu, bool isShowDescription)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            List<WebMenuDto> query = null;
            if (isMenu.HasValue)
            {
                query = list.FindAll(q => q.ParentId == parentId && q.IsMenu == isMenu.Value);
            }
            else
            {
                query = list.FindAll(q => q.ParentId == parentId);
            }
            List<TreeNodeDto> treelist = new List<TreeNodeDto>(query.Count);
            foreach (var vm in query)
            {
                TreeNodeDto m = new TreeNodeDto()
                {
                    id = vm.Id,
                    text = vm.Name,
                    Obj = vm.PageUrl
                };
                if (isShowDescription && !string.IsNullOrEmpty(vm.Description))
                {
                    m.text = $"{vm.Name} - ({vm.Description})";
                }
                var children = this.GetTreeList(vm.Id, list, isMenu, isShowDescription);
                if (children.Count > 0)
                {
                    m.state = "closed";
                    m.children = children;
                }
                treelist.Add(m);
            }

            return treelist;
        }

        [UserAuth]
        public IActionResult GetRoleMenu(string roleId)
        {
            if (!string.IsNullOrEmpty(roleId))
            {
                var data = this.roleWebMenuService.Get(roleId);

                return Success(data);
            }
            return Error();
        }

        [UserAuth("1001001000000")]
        public IActionResult SaveRoleMenu(RoleWebMenuInputDto vm)
        {
            if(vm != null && this.ModelState.IsValid)
            {
                var role = this.roleService.Get(vm.RoleId);
                if (role != null)
                {
                    var arr = (vm.WebMenuIds ?? "").Split(',');
                    var list = new List<string>(arr);
                    var result = this.roleWebMenuService.Update(vm.RoleId, list);
                    var userinfo = this.UserSession;
                    LogUtils.Debug($"【修改角色权限】{userinfo.Name}({userinfo.Account})，角色: {role.Name}！");

                    return Success(result);
                }
            }

            return Error();
        }
    }
}