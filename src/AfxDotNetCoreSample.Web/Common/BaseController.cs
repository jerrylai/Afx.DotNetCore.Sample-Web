using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Web.Http;

using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.IService;

namespace AfxDotNetCoreSample.Web
{
    public abstract class BaseController : Controller
    {
        private Dictionary<Type, IBaseService> serviceDic = new Dictionary<Type, IBaseService>(5);
        protected virtual T GetService<T>(string name, object[] args) where T : IBaseService
        {
            var type = typeof(T);
            IBaseService service = null;
            if (!serviceDic.TryGetValue(type, out service))
            {
                serviceDic[type] = service = IocUtils.Get<T>(name, args);
                service.SetCurrentUser(this.UserSession);
            }

            return (T)service;
        }

        protected virtual T GetService<T>(string name) where T : IBaseService
        {
            return this.GetService<T>(name, null);
        }

        protected virtual T GetService<T>(object[] args) where T : IBaseService
        {
            return this.GetService<T>(null, args);
        }

        protected virtual T GetService<T>() where T : IBaseService
        {
            return this.GetService<T>(null, null);
        }

        protected virtual ActionResult ApiResult<T>(ApiStatus status, T data, string msg)
        {
            var vm = new ApiResult<T>()
            {
                Status = status,
                Data = data,
                Msg = msg
            };

            if (this.Request.IsIFrameAjax()) return this.Content(JsonUtils.Serialize(vm));

            return this.Json(vm);
        }

        protected virtual ActionResult Success<T>(T data, string msg)
        {
            return this.ApiResult<T>(ApiStatus.Success, data, msg);
        }

        protected virtual ActionResult Success<T>(T data)
        {
            return this.ApiResult<T>(ApiStatus.Success, data, null);
        }

        protected virtual ActionResult Success()
        {
            return this.ApiResult<object>(ApiStatus.Success, null, null);
        }

        protected virtual ActionResult Error()
        {
            return this.Failure(ApiStatus.Error);
        }

        protected virtual ActionResult Error(string msg)
        {
            return this.Failure(ApiStatus.Error, msg);
        }

        protected virtual ActionResult Failure()
        {
            return this.ApiResult<object>(ApiStatus.Failure, null, null);
        }

        protected virtual ActionResult Failure(string msg)
        {
            return this.ApiResult<object>(ApiStatus.Failure, null, msg);
        }

        protected virtual ActionResult Failure(ApiStatus status)
        {
            return this.ApiResult<object>(status, null, null);
        }

        protected virtual ActionResult Failure(ApiStatus status, string msg)
        {
            return this.ApiResult<object>(status, null, msg);
        }

        protected virtual string Sid => this.HttpContext.GetSid();

        protected virtual UserSessionDto UserSession
        {
            get
            {
                return this.HttpContext.GetUserSession();
            }
            set
            {
                this.HttpContext.SetUserSession(value);
            }
        }

        protected virtual string CurrentUserId
        {
            get { return this.UserSession?.UserId; }
        }

        protected virtual string CurrentRoleId
        {
            get { return this.UserSession?.RoleId; }
        }

        protected virtual void ClearUserSession()
        {
            this.HttpContext.ClearUserSession();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.serviceDic != null)
            {
                foreach (var kv in this.serviceDic)
                {
                    kv.Value.Dispose();
                }
                this.serviceDic.Clear();
                this.serviceDic = null;
            }

            base.Dispose(disposing);
        }
    }
}
