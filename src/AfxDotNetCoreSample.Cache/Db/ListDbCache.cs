﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfxDotNetCoreSample.Cache
{
    public abstract class ListDbCache<TCache> : BaseCache<TCache>
    {
        public ListDbCache() : base("ListDb") { }
    }
}
