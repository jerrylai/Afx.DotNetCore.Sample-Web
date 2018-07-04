﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class ConfigCache : DataDbCache, IConfigCache
    {
        public virtual List<ConfigDto> Get(ConfigType type)
        {
            return base.Get<List<ConfigDto>>(type);
        }

        public virtual void Remove(ConfigType type)
        {
            base.Remove(type);
        }

        public virtual void Set(ConfigType type, List<ConfigDto> list)
        {
            base.Set(list, type);
        }
    }
}
