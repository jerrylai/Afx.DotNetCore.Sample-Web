using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.IService;

namespace AfxDotNetCoreSample.Web.Controllers
{
    [UserAuth]
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            var userInfo = this.UserSession;
            ViewBag.UserName = userInfo.Name;

            return View();
        }

        public IActionResult Main()
        {
            return View();
        }
    }
}
