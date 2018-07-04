using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Enums;
using Afx.Utils;

namespace AfxDotNetCoreSample.Dto
{
    public class ApiException : Exception
    {
        public ApiStatus Status { get; set; }

        public ApiException(ApiStatus status, string message)
            : base(message)
        {
            this.Status = status;
        }

        public ApiException()
            : this(ApiStatus.Failure)
        {
        }

        public ApiException(ApiStatus status)
            : this(status, status.GetDescription())
        {

        }

        public ApiException(string message)
            : this(ApiStatus.Failure, message)
        {

        }

    }
}
