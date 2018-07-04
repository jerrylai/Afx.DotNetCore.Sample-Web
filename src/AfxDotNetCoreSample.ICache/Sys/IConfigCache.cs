using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.ICache
{
    public interface IConfigCache : IBaseCache
    {
        List<ConfigDto> Get(ConfigType type);
        void Set(ConfigType type, List<ConfigDto> list);
        void Remove(ConfigType type);
    }
}
