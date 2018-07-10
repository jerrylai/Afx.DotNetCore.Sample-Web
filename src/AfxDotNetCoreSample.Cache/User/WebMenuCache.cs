using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class WebMenuCache : ListDbCache, IWebMenuCache
    {
        public virtual List<WebMenuOutputDto> Get()
        {
            return base.Get<List<WebMenuOutputDto>>();
        }

        public virtual void Remove()
        {
            base.Remove();
        }

        public virtual void Set(List<WebMenuOutputDto> list)
        {
            base.Set(list);
        }
    }
}
