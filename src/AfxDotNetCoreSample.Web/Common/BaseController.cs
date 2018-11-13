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
        protected virtual ActionResult ApiResult<T>(ApiStatus status, T data, string msg)
        {
            var vm = new ApiResult<T>()
            {
                Status = status,
                Data = data,
                Msg = msg
            };

            return base.Json(vm);
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
    }
}
