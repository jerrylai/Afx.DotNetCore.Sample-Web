using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afx.Utils;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Common
{
    public class ApiException : Exception
    {
        public ApiStatus Status { get; set; }

        public ApiException(ApiStatus status, string message)
            : base(message)
        {
            this.Status = status;
        }

        public ApiException(ApiStatus status)
            : this(status, status.GetDescription())
        {
            this.Status = status;
        }

        public ApiException()
            : this(ApiStatus.Failure)
        {
        }
        
        public ApiException(string message)
            : this(ApiStatus.Failure, message)
        {

        }

    }

    public class ApiParamNullException : ApiException
    {
        public ApiParamNullException(string paramName) : base(ApiStatus.Error, $"参数({paramName})不能为空！")
        {

        }
    }

    public class ApiParamException : ApiException
    {
        public ApiParamException(string paramName) : base(ApiStatus.Error, $"参数({paramName})不正确！")
        {

        }
    }
}
