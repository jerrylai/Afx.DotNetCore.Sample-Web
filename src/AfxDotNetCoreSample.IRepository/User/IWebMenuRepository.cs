using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IRepository
{
    public interface IWebMenuRepository : IBaseRepository
    {
        List<WebMenuDto> GetList();
    }
}
