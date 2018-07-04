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
        public virtual List<WebMenuDto> GetList()
        {
            var repository = this.GetRepository<IWebMenuRepository>();
            var list = repository.GetList();

            return list;
        }

        public virtual List<TreeNodeDto> GetTreeNodeList()
        {
            var list = this.GetList();
            var nodelist = this.GetChildren(null, list);

            return nodelist;
        }

        public virtual List<TreeNodeDto> GetTreeNodeList(string roleId)
        {
            var list = this.GetList();

            var repository = this.GetRepository<IRoleWebMenuRepository>();
            var roleauths = repository.Get(roleId);
            list.RemoveAll(q => !roleauths.Contains(q.Id));

            var nodelist = this.GetChildren(null, list);

            return nodelist;
        }

        private List<TreeNodeDto> GetChildren(string parentId, List<WebMenuDto> list)
        {
            List<TreeNodeDto> child = null;
            var query = from q in list
                        where q.ParentId == parentId
                        select new TreeNodeDto
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
