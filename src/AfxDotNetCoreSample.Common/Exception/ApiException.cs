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
}
