using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;

namespace AfxDotNetCoreSample.Repository
{
    public class WebMenuRepository : BaseRepository, IWebMenuRepository
    {
        public virtual List<WebMenuDto> GetList()
        {
            List<WebMenuDto> list = null;
            var cache = this.GetCache<IWebMenuCache>();
            list = cache.Get();
            if (list == null)
            {
                using(var db = this.GetContext())
                {
                    var query = from q in db.WebMenu
                                orderby q.Order, q.Id
                                select new WebMenuDto
                                {
                                    Id = q.Id,
                                    ParentId = q.ParentId,
                                    Name = q.Name,
                                    Order = q.Order,
                                    Url = q.Url,
                                    IsMenu = q.IsMenu
                                };
                    list = query.ToList();
                    cache.Set(list);
                }
            }

            return list;
        }
    }
}
