using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace AfxDotNetCoreSample.Dto
{
    public class LoginInputDto
    {
        [Required]
        public string Account { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Random { get; set; }
    }
}
