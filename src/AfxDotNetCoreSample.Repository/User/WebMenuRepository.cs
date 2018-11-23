using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Common;

namespace AfxDotNetCoreSample.Repository
{
    public class WebMenuRepository : BaseRepository, IWebMenuRepository
    {
        protected virtual IWebMenuCache cache => this.GetCache<IWebMenuCache>();


        public virtual List<WebMenuDto> GetList()
        {
            var list = this.cache.Get();
            if (list == null)
            {
                using(var db = this.GetContext())
                {
                    var query = from q in db.WebMenu
                                select new WebMenuDto
                                {
                                    Id = q.Id,
                                    ParentId = q.ParentId,
                                    Name = q.Name,
                                    Order = q.Order,
                                    PageUrl = q.PageUrl,
                                    ImageUrl = q.ImageUrl,
                                    IsMenu = q.IsMenu,
                                    Description = q.Description
                                };
                    list = query.ToList();
                    this.cache.Set(list);
                }
            }

            return list;
        }
    }
}
