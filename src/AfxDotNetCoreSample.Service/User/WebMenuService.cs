using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.Common;


namespace AfxDotNetCoreSample.Service
{
    public class WebMenuService : BaseService, IWebMenuService
    {
        protected virtual IWebMenuRepository repository => this.GetRepository<IWebMenuRepository>();

        public virtual List<WebMenuDto> GetList()
        {
            var list = this.repository.GetList();

            return list;
        }

    }
}
