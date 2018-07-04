using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Dto
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class ConfigDto
    {
        public string Id { get; set; }

        public ConfigType Type { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
