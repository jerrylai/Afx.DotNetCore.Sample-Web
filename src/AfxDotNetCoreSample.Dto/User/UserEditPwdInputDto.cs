using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace AfxDotNetCoreSample.Dto
{
    public class UserEditPwdInputDto
    {
        public string UserId { get; set; }

        [Required]
        public string OldPwd { get; set; }

        [Required]
        public string NewPwd { get; set; }
    }
}
