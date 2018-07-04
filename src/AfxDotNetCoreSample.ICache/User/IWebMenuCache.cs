using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.ICache
{
    public interface IWebMenuCache : IBaseCache
    {
        List<WebMenuDto> Get();
        void Set(List<WebMenuDto> list);
        void Remove();
    }
}
