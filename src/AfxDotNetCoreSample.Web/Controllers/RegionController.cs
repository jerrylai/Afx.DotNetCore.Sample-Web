using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Web.Controllers
{
    public class RegionController : BaseController
    {
        private readonly Lazy<IRegionService> regionService = new Lazy<IRegionService>(IocUtils.Get<IRegionService>);

        [UserAuth("1002001000000")]
        public IActionResult Index()
        {
            return View();
        }

        [UserAuth]
        public IActionResult GetTreeList(string id)
        {
            var treelist = this.GetTree(id, false);

            return Json(treelist);
        }

        [UserAuth]
        public IActionResult GetTreeListFullName(string id)
        {
            var treelist = this.GetTree(id, true);

            return Json(treelist);
        }

        [UserAuth]
        private List<TreeNodeDto> GetTree(string id, bool isFullName)
        {
            id = !string.IsNullOrEmpty(id) ? id : null;
            var list = this.regionService.Value.GetChildList(id);
            var treelist = list.Select(q => new TreeNodeDto
            {
                id = q.Id,
                text = q.Name
            }).ToList();

            foreach (var tree in treelist)
            {
                var childids = this.regionService.Value.GetChildId(tree.id);
                if (childids.Count > 0) tree.state = "closed";
                if(isFullName) tree.showText = this.regionService.Value.GetFullName(tree.id, "/");
            }

            return treelist;
        }

        [UserAuth("1002001000000")]
        public IActionResult Delete(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                var vm = this.regionService.Value.Get(id);
                if (vm != null)
                {
                    var userinfo = this.UserSession;
                    bool result = this.regionService.Value.Delete(id);
                    LogUtils.Debug($"【删除地区】{userinfo.Name}({userinfo.Account}), 删除 {vm.Name} 成功！");

                    return Success(result);
                }
            }

            return Error();
        }

        [HttpPost]
        [UserAuth("1002001000000")]
        public IActionResult Add(RegionDto vm)
        {
            if (vm != null && this.ModelState.IsValid)
            {
                var userinfo = this.UserSession;
                bool result = this.regionService.Value.Add(vm);
                LogUtils.Debug($"【添加地区】{userinfo.Name}({userinfo.Account}), 添加 {vm.Name} 成功！");

                return Success(result);
            }

            return Error();
        }

        [HttpPost]
        [UserAuth("1002001000000")]
        public IActionResult Update(RegionDto vm)
        {
            if (vm != null && this.ModelState.IsValid && !string.IsNullOrEmpty(vm.Id))
            {
                var m = this.regionService.Value.Get(vm.Id);
                if (m != null)
                {
                    var userinfo = this.UserSession;
                    bool result = this.regionService.Value.Update(vm);
                    LogUtils.Debug($"【修改地区】{userinfo.Name}({userinfo.Account}), 修改 {m.Name} -> {vm.Name} 成功！");

                    return Success(vm.Name);
                }
            }

            return Error();
        }

        [UserAuth]
        public IActionResult GetFullName(string id)
        {
            string fullname = "";
            if(!string.IsNullOrEmpty(id))
            {
                fullname = this.regionService.Value.GetFullName(id, "/");
            }

            return Success(fullname);
        }
    }
}