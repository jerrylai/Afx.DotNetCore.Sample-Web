using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Dto
{
    public class UserPageInputDto : PageDataInputDto
    {
        public string RoleId { get; set; }

        public UserStatus? Status { get; set; }

    }
}
