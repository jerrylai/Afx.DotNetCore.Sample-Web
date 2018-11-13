using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Dto
{
    public class UserExistDto
    {
        public string Id { get; set; }

        public UserIdCacheType Type { get; set; }

        [Required]
        public string Key { get; set; }
    }
}
