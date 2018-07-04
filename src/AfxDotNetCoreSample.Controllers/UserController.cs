﻿using Microsoft.AspNetCore.Mvc;
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
    public class UserController : BaseController
    {
        [HttpPost]
        public ActionResult Login(LoginParamDto vm)
        {
            if(vm != null && !string.IsNullOrEmpty(vm.Account)
                && !string.IsNullOrEmpty(vm.Password) && !string.IsNullOrEmpty(vm.Random))
            {
                var userService = this.GetService<IUserService>();
                var userinfo = userService.Login(vm);
                if(userinfo != null)
                {
                    this.UserInfo = new UserInfo()
                    {
                        UserId = userinfo.Id,
                        Account = userinfo.Account,
                        Name = userinfo.Name,
                        RoleId = userinfo.RoleId,
                        LoginTime = DateTime.Now
                    };

                    return Success(true);
                }

                return Failure();
            }

            return ParamError();
        }

        [HttpGet, HttpPost]
        public ActionResult IsLogin()
        {
            return Success(this.UserInfo != null);
        }

        [HttpGet, HttpPost]
        public ActionResult Logout()
        {
            this.AbortSession();
            this.SignOut();

            return this.Success();
        }

        [UserAuth]
        [HttpGet, HttpPost]
        public ActionResult GetName()
        {
            return Success(this.UserInfo.Name);
        }
    }
}