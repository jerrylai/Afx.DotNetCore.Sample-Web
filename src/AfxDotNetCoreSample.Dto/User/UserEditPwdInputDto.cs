using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.Dto
{
    public class UserEditPwdInputDto
    {
        public string UserId { get; set; }

        public string OldPwd { get; set; }

        public string NewPwd { get; set; }
    }
}
