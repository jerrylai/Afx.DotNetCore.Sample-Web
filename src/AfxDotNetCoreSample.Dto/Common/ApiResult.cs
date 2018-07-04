using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Dto
{
    public class ApiResult<T>
    {
        public ApiStatus Status { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }
    }

    public class ApiResult : ApiResult<object>
    {

    }
}
