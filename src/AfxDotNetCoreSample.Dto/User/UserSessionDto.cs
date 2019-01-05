using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Dto
{
    public class UserSessionDto
    {
        public string Sid { get; set; }

        public string UserId { get; set; }

        public string RoleId { get; set; }

        public string Account { get; set; }

        public string Name { get; set; }

        public DateTime LoginTime { get; set; }

    }
}
