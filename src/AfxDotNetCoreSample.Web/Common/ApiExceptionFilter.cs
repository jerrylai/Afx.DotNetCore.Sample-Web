using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using Afx.Utils;

namespace AfxDotNetCoreSample.Web
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var method = context.HttpContext.Request.Method;
            var path = context.HttpContext.Request.Path.ToString();
            if (context.Exception is ApiException)
            {
                var ex = context.Exception as ApiException;
                string errormsg = !string.IsNullOrEmpty(ex.Message) ? ex.Message : ex.Status.GetDescription();
                if (context.HttpContext.Request.IsAjax())
                {
                    context.Result = new JsonResult(new ApiResult()
                    {
                        Status = ex.Status,
                        Msg = errormsg
                    });
                }
                else
                {
                    context.Result = new OkObjectResult(errormsg);
                }
                LogUtils.Info($"【ApiExceptionFilter】Method: {method}, url: {path}", ex);
            }
            else
            {
                var ex = context.Exception;
                var errormsg = !string.IsNullOrEmpty(ex.Message) ? ex.Message : ApiStatus.ServerError.GetDescription();
                if (context.HttpContext.Request.IsAjax())
                {
                    context.Result = new JsonResult(new ApiResult()
                    {
                        Status = ApiStatus.ServerError,
                        Msg = errormsg
                    });
                }
                else
                {
                    context.Result = new OkObjectResult(errormsg);
                }

                LogUtils.Error($"【ApiExceptionFilter】Method: {method}, url: {path}", ex);
            }

        }
    }
}
