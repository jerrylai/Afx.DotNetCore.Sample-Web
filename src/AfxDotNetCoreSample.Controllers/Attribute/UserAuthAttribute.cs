using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.IService;

using Afx.Utils;
using Afx.Ioc;

namespace AfxDotNetCoreSample.Controllers
{
    /// <summary>
    /// 授权验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class UserAuthAttribute : Attribute, IAuthorizationFilter
    {
        public string ActionId { get; private set; }

        public UserAuthAttribute()
        {

        }

        public UserAuthAttribute(string actionId)
        {
            this.ActionId = actionId;
        }
                
        /// <summary>
        /// 设置返回错误
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="status"></param>
        /// <param name="msg"></param>
        private void SetError(AuthorizationFilterContext actionContext, ApiStatus status, string msg)
        {
            actionContext.Result = new JsonResult(
                new ApiResult()
                {
                    Status = status,
                    Msg = msg ?? ApiStatus.NeedAuth.GetDescription()
                });

            var url = actionContext.HttpContext.Request.Path;
            LogUtils.Debug($"【Auth】url: {url}, " + (msg ?? ApiStatus.NeedAuth.GetDescription()));
        }

        public virtual void OnAuthorization(AuthorizationFilterContext context)
        {
           var actionDescriptor =  context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            if(actionDescriptor != null)
            {
                var arr = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AnonymousAttribute), true);
                if (arr != null && arr.Length > 0) return;
            }

            var user = IocUtils.Get<IUserSessionService>().Get();
            if (user == null)
            {
                this.SetError(context, ApiStatus.NeedLogin, "未登录或登录已超时！");
            }
            else if (!string.IsNullOrEmpty(this.ActionId))
            {
                var servce = IocUtils.Get<IRoleWebMenuService>();
                if (servce.Exist(user.RoleId, this.ActionId))
                {
                    this.SetError(context, ApiStatus.NeedAuth, "无权限访问！");
                }
            }
        }

    }
}
