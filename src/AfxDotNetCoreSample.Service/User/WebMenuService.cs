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
        private readonly Lazy<IWebMenuRepository> _repository = new Lazy<IWebMenuRepository>(() => IocUtils.Get<IWebMenuRepository>());
        internal protected virtual IWebMenuRepository repository => this._repository.Value;

        public virtual List<WebMenuDto> GetList()
        {
            var list = this.repository.GetList();

            return list;
        }

    }
}
