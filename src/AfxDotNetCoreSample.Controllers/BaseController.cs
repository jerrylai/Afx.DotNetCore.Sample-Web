using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Web.Http;

using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.IService;

namespace AfxDotNetCoreSample.Controllers
{
    public abstract class BaseController : Controller
    {
        protected virtual T GetService<T>() where T : IBaseService => IocUtils.Get<T>();

        protected virtual T GetService<T>(string name) where T : IBaseService => IocUtils.Get<T>(name);

        protected virtual T GetService<T>(object[] args) where T : IBaseService => IocUtils.Get<T>(args);

        protected virtual T GetService<T>(string name, object[] args) where T : IBaseService => IocUtils.Get<T>(name, args);
        
        protected ActionResult ApiResult<T>(ApiStatus status, T data, string msg)
        {
            var vm = new ApiResult<T>()
            {
                Status = status,
                Data = data,
                Msg = msg
            };

            return base.Json(vm);
        }

        protected ActionResult Success<T>(T data, string msg)
        {
            return this.ApiResult<T>(ApiStatus.Success, data, msg);
        }

        protected ActionResult Success<T>(T data)
        {
            return this.ApiResult<T>(ApiStatus.Success, data, null);
        }

        protected ActionResult Success()
        {
            return this.ApiResult<object>(ApiStatus.Success, null, null);
        }

        protected ActionResult ParamError()
        {
            return this.Failure(ApiStatus.ParamError);
        }

        protected ActionResult Failure()
        {
            return this.ApiResult<object>(ApiStatus.Failure, null, null);
        }

        protected ActionResult Failure(string msg)
        {
            return this.ApiResult<object>(ApiStatus.Failure, null, msg);
        }

        protected ActionResult Failure(ApiStatus status)
        {
            return this.ApiResult<object>(status, null, null);
        }

        protected ActionResult Failure(ApiStatus status, string msg)
        {
            return this.ApiResult<object>(status, null, msg);
        }

        protected virtual string Sid => SessionUtils.Sid;

        protected virtual UserInfo UserInfo
        {
            get { return SessionUtils.User; }
            set { SessionUtils.User = value; }
        }

        protected virtual void SetSession<T>(string key, T value) => SessionUtils.Set(key, value);
        
        protected virtual T GetSession<T>(string key) => SessionUtils.Get<T>(key);

        protected virtual void RemoveSession(string key) => SessionUtils.Remove(key);

        protected virtual void ClearSession() => SessionUtils.Clear();

        protected virtual List<string> GetSessionKeys() => SessionUtils.GetKeys();

        protected virtual void AbortSession()
        {
            SessionUtils.Logout();
        }
        
    }
}
