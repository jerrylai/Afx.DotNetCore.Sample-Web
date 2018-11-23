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
using Afx.Utils;

namespace AfxDotNetCoreSample.Web.Controllers
{
    public class UserController : BaseController
    {
        private IUserService userService => this.GetService<IUserService>();
        
        [HttpGet]
        [Anonymous]
        public ActionResult Login()
        {
            if(this.UserSession != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [Anonymous]
        [HttpPost]
        public ActionResult Login(LoginInputDto vm)
        {
            if(vm != null && !string.IsNullOrEmpty(vm.Account)
                && !string.IsNullOrEmpty(vm.Password) && !string.IsNullOrEmpty(vm.Random))
            {
                var userinfo = userService.Login(vm);
                if(userinfo != null)
                {
                    this.UserSession = new UserSessionDto()
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

            return Error();
        }
        
        [HttpGet]
        [Anonymous]
        public ActionResult Logout()
        {
            this.ClearUserSession();

            return RedirectToAction("Login");
        }
        
        [HttpGet]
        [UserAuth]
        public ActionResult EditPwd()
        {

            return View();
        }

        [HttpPost]
        [UserAuth]
        public ActionResult EditPwd(UserEditPwdInputDto vm)
        {
            if(vm != null && this.ModelState.IsValid)
            {
                var userinfo = this.UserSession;
                vm.UserId = userinfo.UserId;
                bool result = this.userService.EditPwd(vm);
                LogUtils.Debug($"【修改个人密码】{userinfo.Name}({userinfo.Account}) 修改密码成功！");

                return Success(result);
            }

            return Error();
        }

        [UserAuth("1001002000000")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [UserAuth]
        public IActionResult GetPageData(UserPageInputDto vm)
        {
            if (vm != null && this.ModelState.IsValid)
            {
                var data = this.userService.GetPageData(vm);

                return Success(data);
            }

            return Error();
        }

        [HttpPost]
        [UserAuth("1001002000000")]
        public ActionResult Edit(UserDto vm)
        {
            if(vm != null && this.ModelState.IsValid)
            {
                var userinfo = this.UserSession;
                if (!string.IsNullOrEmpty(vm.Id))
                {
                    this.userService.Update(vm);
                    LogUtils.Debug($"【修改账号】{userinfo.Name}({userinfo.Account})，修改 {vm.Name}({vm.Account}) 成功！");
                }
                else
                {
                    if (string.IsNullOrEmpty(vm.Account)) return Error("账号不能为空！");
                    if (string.IsNullOrEmpty(vm.Password)) return Error("密码不能为空！");
                    this.userService.Add(vm);
                    LogUtils.Debug($"【添加账号】{userinfo.Name}({userinfo.Account})，添加 {vm.Name}({vm.Account}) 成功！");
                }

                return Success(true);
            }

            return Error();
        }

        [UserAuth("1001002000000")]
        public ActionResult Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var m = this.userService.Get(id);
                if (m != null)
                {
                    var userinfo = this.UserSession;
                    bool result = this.userService.Delete(id);
                    LogUtils.Debug($"【删除账号】{userinfo.Name}({userinfo.Account})，删除 {m.Name}({m.Account}) 成功！");
                    return Success(result);
                }
            }
            return Error();
        }

        [Anonymous]
        public IActionResult IsExist(UserExistDto vm)
        {
            if (vm != null && this.ModelState.IsValid)
            {
                string id = null;
                switch (vm.Type)
                {
                    case UserIdCacheType.Account:
                        id = this.userService.GetId(vm.Key);
                        break;
                    case UserIdCacheType.Mail:
                        id = this.userService.GetIdByMail(vm.Key);
                        break;
                    case UserIdCacheType.Mobile:
                        id = this.userService.GetIdByMobile(vm.Key);
                        break;
                }
                var result = !(string.IsNullOrEmpty(id)
                     ||  !string.IsNullOrEmpty(vm.Id) && !string.IsNullOrEmpty(id) && vm.Id == id);

                return Success(result);
            }

            return Error();
        }

        [HttpGet]
        [UserAuth]
        public IActionResult MyInfo()
        {
            var userinfo = this.UserSession;
            var m = this.userService.Get(userinfo.UserId);

            return View(m);
        }

        [HttpPost]
        [UserAuth]
        public IActionResult MyInfo(UserDto vm)
        {
            if (vm != null)
            {
                var userinfo = this.UserSession;
                var m = this.userService.Get(userinfo.UserId);
                m.Name = vm.Name;
                m.Mail = vm.Mail;
                m.Mobile = vm.Mobile;
                this.userService.Update(m);
                userinfo.Name = vm.Name;
                this.UserSession = userinfo;

                LogUtils.Debug($"【修改个人信息】{userinfo.Name}({userinfo.Account}) 成功！");

                return Success(vm);
            }

            return Error();
        }
    }
}