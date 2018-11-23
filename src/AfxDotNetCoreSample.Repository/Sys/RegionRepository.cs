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
    public class RegionRepository : BaseRepository, IRegionRepository
    {
        protected virtual IRegionCache regionCache => this.GetCache<IRegionCache>();

        protected virtual IRegionChildCache regionChildCache => this.GetCache<IRegionChildCache>();

        protected virtual RegionLevelRepository regionLevelRepository => this.GetRepository<IRegionLevelRepository>() as RegionLevelRepository;

        public virtual int Add(RegionDto vm)
        {
            int count = 0;
            Region m = new Region()
            {
                Id = IdGenerator.Get<Region>(),
                Name = vm.Name,
                Level = vm.Level,
                ParentId = vm.ParentId,
                IsDelete = false
            };
            using(var db = this.GetContext())
            {
                using (db.BeginTransaction())
                {
                    db.AddCommitCallback((num) =>
                    {
                        this.regionChildCache.Remove(m.ParentId);
                    });
                    db.Region.Add(m);
                    count = db.SaveChanges();
                    this.regionLevelRepository.Add(m.Id, m.Level, m.ParentId, db);
                    db.Commit();
                }
            }

            return count;
        }

        public virtual int Delete(string id)
        {
            var count = 0;
            using (var db = this.GetContext())
            {
                var m = db.Region.Where(q => q.Id == id && q.IsDelete == false).FirstOrDefault();
                if (m != null)
                {
                    db.AddCommitCallback((num) => {
                        this.regionCache.Remove(m.Id);
                        this.regionChildCache.Remove(m.ParentId);
                    });
                    db.Region.Remove(m);
                    count = db.SaveChanges();
                }
            }

            return count;
        }

        public virtual RegionDto Get(string id)
        {
            RegionDto vm = this.regionCache.Get(id);
            if(vm == null)
            {
                using(var db = this.GetContext())
                {
                    var query = from q in db.Region
                                where q.Id == id && q.IsDelete == false
                                select new RegionDto
                                {
                                    Id = q.Id,
                                    Name = q.Name,
                                    Level = q.Level,
                                    ParentId = q.ParentId
                                };
                    vm = query.FirstOrDefault();
                }

                if (vm != null) this.regionCache.Set(id, vm);
            }

            return vm;
        }

        public virtual List<string> GetChild(string parentId)
        {
            List<string> list = this.regionChildCache.Get(parentId);
            if (list == null)
            {
                using (var db = this.GetContext())
                {
                    var query = from q in db.Region
                                where q.ParentId == parentId && q.IsDelete == false
                                select q.Id;
                    list = query.ToList();
                }

                this.regionChildCache.Set(parentId, list);
            }

            return list;
        }

        public virtual int Update(RegionDto vm)
        {
            var count = 0;
            using (var db = this.GetContext())
            {
                var m = db.Region.Where(q => q.Id == vm.Id && q.IsDelete == false).FirstOrDefault();
                if (m != null)
                {
                    db.AddCommitCallback((num) => {
                        this.regionCache.Remove(m.Id);
                    });
                    m.Name = vm.Name;
                    count = db.SaveChanges();
                }
            }

            return count;
        }
    }
}
