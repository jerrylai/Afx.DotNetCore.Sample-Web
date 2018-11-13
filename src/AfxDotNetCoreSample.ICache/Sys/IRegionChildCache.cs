using System;
using System.Collections.Generic;
using System.Text;

namespace AfxDotNetCoreSample.ICache
{
    public interface IRegionChildCache : IBaseCache
    {
        List<string> Get(string parentId);
        void Set(string parentId, List<string> list);
        void Remove(string parentId);
    }
}
