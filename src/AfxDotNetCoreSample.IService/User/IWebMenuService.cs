using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IService
{
    public interface IWebMenuService : IBaseService
    {
        List<WebMenuDto> GetList();
        List<TreeNodeDto> GetTreeNodeList();
        List<TreeNodeDto> GetTreeNodeList(string roleId);
    }
}
