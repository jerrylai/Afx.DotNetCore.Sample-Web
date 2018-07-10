using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.ICache
{
    public interface IWebMenuCache : IBaseCache
    {
        List<WebMenuOutputDto> Get();
        void Set(List<WebMenuOutputDto> list);
        void Remove();
    }
}
