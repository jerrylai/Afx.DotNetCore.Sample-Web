using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace AfxDotNetCoreSample.Dto
{
    public class RoleWebMenuInputDto
    {
        [Required]
        public string RoleId { get; set; }

        public string WebMenuIds { get; set; }
    }
}
