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
                context.Result = new JsonResult(new ApiResult()
                    {
                        Status = ex.Status,
                        Msg = !string.IsNullOrEmpty(ex.Message) ? ex.Message : ex.Status.GetDescription()
                    });

                LogUtils.Info($"【ApiExceptionFilter】Method: {method}, url: {path}", ex);
            }
            else
            {
                var ex = context.Exception;
                context.Result = new JsonResult(new ApiResult()
                {
                    Status =  ApiStatus.ServerError,
                    Msg = !string.IsNullOrEmpty(ex.Message) ? ex.Message : ApiStatus.ServerError.GetDescription()
                });

                LogUtils.Error("【ApiExceptionFilter】Method: {method}, url: {path}", ex);
            }

        }
    }
}
