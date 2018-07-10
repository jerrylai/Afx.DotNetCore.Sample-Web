using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IService
{
    public interface IWebMenuService : IBaseService
    {
        List<WebMenuOutputDto> GetList();
        List<TreeNodeOutputDto> GetTreeNodeList();
        List<TreeNodeOutputDto> GetTreeNodeList(string roleId);
    }
}
