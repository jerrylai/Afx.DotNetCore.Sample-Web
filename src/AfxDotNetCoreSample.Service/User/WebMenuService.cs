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
        public virtual List<WebMenuOutputDto> GetList()
        {
            var repository = this.GetRepository<IWebMenuRepository>();
            var list = repository.GetList();

            return list;
        }

        public virtual List<TreeNodeOutputDto> GetTreeNodeList()
        {
            var list = this.GetList();
            var nodelist = this.GetChildren(null, list);

            return nodelist;
        }

        public virtual List<TreeNodeOutputDto> GetTreeNodeList(string roleId)
        {
            var list = this.GetList();

            var repository = this.GetRepository<IRoleWebMenuRepository>();
            var roleauths = repository.Get(roleId);
            list.RemoveAll(q => !roleauths.Contains(q.Id));

            var nodelist = this.GetChildren(null, list);

            return nodelist;
        }

        private List<TreeNodeOutputDto> GetChildren(string parentId, List<WebMenuOutputDto> list)
        {
            List<TreeNodeOutputDto> child = null;
            var query = from q in list
                        where q.ParentId == parentId
                        select new TreeNodeOutputDto
                        {
                            id = q.Id,
                            text = q.Name,
                            url = q.Url
                        };
            child = query.ToList();
            if (child.Count > 0)
            {
                foreach (var ch in child)
                {
                    ch.children = this.GetChildren(ch.id, list);
                    ch.state = ch.children != null ? "closed" : null;
                }
            }
            else
            {
                child = null;
            }

            return child;
        }
    }
}
