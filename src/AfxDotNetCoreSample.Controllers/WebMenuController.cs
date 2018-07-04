using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.IService;

namespace AfxDotNetCoreSample.Controllers
{
    [UserAuth]
    public class WebMenuController : BaseController
    {
        [HttpGet, HttpPost]
        public ActionResult GetUserMenu()
        {
            var service = this.GetService<IWebMenuService>();
            var list = service.GetTreeNodeList(this.UserInfo.RoleId);

            return Success(list);
        }
    }
}