using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using AfxDotNetCoreSample.Common;

namespace AfxDotNetCoreSample.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet, HttpPost]
        public ActionResult Index()
        {
            if (this.UserSession != null)
            {
                return this.LocalRedirect($"~/main.html{this.Request.QueryString}");
            }

            return this.LocalRedirect($"~/index.html{this.Request.QueryString}");
        }
    }
}