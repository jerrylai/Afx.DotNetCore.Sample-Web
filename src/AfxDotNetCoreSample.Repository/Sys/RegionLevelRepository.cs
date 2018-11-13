using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Models;
using AfxDotNetCoreSample.Common;

namespace AfxDotNetCoreSample.Repository
{
    public class RegionLevelRepository : BaseRepository, IRegionLevelRepository
    {
        private readonly Lazy<IRegionLevelCache> _regionLevelCache = new Lazy<IRegionLevelCache>(IocUtils.Get<IRegionLevelCache>);
        internal protected virtual IRegionLevelCache regionLevelCache => this._regionLevelCache.Value;

        public virtual List<RegionLevelDto> GetList(string regionId)
        {
            if (string.IsNullOrEmpty(regionId)) return null;
            var list = this.regionLevelCache.Get(regionId);
            if (list == null)
            {
                using (var db = this.GetContext())
                {
                    var query = from q in db.RegionLevel
                                where q.RegionId == regionId 
                                orderby q.ParentLevel
                                select new RegionLevelDto
                                {
                                    Id = q.Id,
                                    RegionId = q.RegionId,
                                    ParentId = q.ParentId,
                                    ParentLevel = q.ParentLevel
                                };
                    list = query.ToList();
                }
                this.regionLevelCache.Set(regionId, list);
            }

            return list;
        }

        public virtual int Add(string regionId, int level, string parentId, AfxContext db)
        {
            int count = 0;
            if (string.IsNullOrEmpty(regionId)) throw new ArgumentNullException(nameof(regionId));
            if (!string.IsNullOrEmpty(parentId))
            {
                var list = this.GetList(parentId);
                var idqueue = new Queue<string>(IdGenerator.GetList<RegionLevel>(list.Count));
                foreach (var vm in list)
                {
                    var pm = new RegionLevel
                    {
                        Id = idqueue.Dequeue(),
                        RegionId = regionId,
                        ParentId = vm.ParentId,
                        ParentLevel = vm.ParentLevel
                    };
                    db.RegionLevel.Add(pm);
                }
            }
            var m = new RegionLevel
            {
                Id = IdGenerator.Get<RegionLevel>(),
                RegionId = regionId,
                ParentId = regionId,
                ParentLevel = level
            };
            db.RegionLevel.Add(m);
            count = db.SaveChanges();

            return count;
        }
    }
}
