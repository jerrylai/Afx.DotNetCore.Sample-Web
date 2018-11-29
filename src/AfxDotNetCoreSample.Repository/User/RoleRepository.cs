using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Models;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Enums;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AfxDotNetCoreSample.Repository
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        protected virtual IRoleCache roleCache => this.GetCache<IRoleCache>();

        public virtual RoleDto Get(string id)
        {
            var vm = this.roleCache.Get(id);
            if(vm == null)
            {
                using(var db = this.GetContext())
                {
                    using (db.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        var query = from q in db.Role
                                    where q.Id == id && q.IsDelete == false
                                    select new RoleDto
                                    {
                                        Id = q.Id,
                                        IsSystem = q.IsSystem,
                                        Name = q.Name,
                                        CreateTime = q.CreateTime,
                                        UpdateTime = q.UpdateTime
                                    };
                        vm = query.FirstOrDefault();
                        db.Commit();
                    }
                }
                if (vm != null) this.roleCache.Set(id, vm);
            }

            return vm;
        }

        public virtual int Add(RoleDto vm)
        {
            int count = 0;
            var m = new Role
            {
                Id = IdGenerator.Get<Role>(),
                IsSystem = false,
                IsDelete = false,
                Name = vm.Name
            };
            using (var db = this.GetContext())
            {
                using (db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    db.Role.Add(m);
                    count = db.SaveChanges();
                    db.Commit();
                }
                vm.UpdateTime = m.UpdateTime;
                vm.CreateTime = m.CreateTime;
            }

            return count;
        }

        public virtual int Update(RoleDto vm)
        {
            int count = 0;
            using(var db = this.GetContext())
            {
                using (db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var m = db.Role.Where(q => q.Id == vm.Id && q.IsDelete == false).FirstOrDefault();
                    if (m != null)
                    {
                        m.Name = vm.Name;
                        db.AddCommitCallback((num) => this.roleCache.Remove(m.Id));
                        count = db.SaveChanges();
                        vm.UpdateTime = m.UpdateTime;
                        vm.CreateTime = m.CreateTime;
                    }
                    db.Commit();
                }
            }

            return count;
        }

        public virtual int Delete(string id)
        {
            int count = 0;
            using (var db = this.GetContext())
            {
                using (db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var m = db.Role.Where(q => q.Id == id && q.IsSystem == false && q.IsDelete == false).FirstOrDefault();
                    if (m != null)
                    {
                        db.Role.Remove(m);
                        db.AddCommitCallback((num) => this.roleCache.Remove(m.Id));
                        count = db.SaveChanges();
                    }
                    db.Commit();
                }
            }

            return count;
        }

        public virtual PageDataOutputDto<RoleDto> GetPage(RolePageInputDto vm)
        {
            PageDataOutputDto<RoleDto> pageData = null;
            using(var db = this.GetContext())
            {
                using (db.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    var query = from q in db.Role
                                where q.IsDelete == false
                                select new RoleDto
                                {
                                    Id = q.Id,
                                    IsSystem = q.IsSystem,
                                    Name = q.Name,
                                    CreateTime = q.CreateTime,
                                    UpdateTime = q.UpdateTime
                                };

                    if (!string.IsNullOrEmpty(vm.Id))
                    {
                        query = query.Where(q => q.Id == vm.Id);
                    }

                    if (!string.IsNullOrEmpty(vm.Keyword))
                    {
                        var value = vm.Keyword.DbLike(DbLikeType.All);
                        query = query.Where(q => EF.Functions.Like(q.Name, value));
                    }

                    pageData = query.ToPage(vm);
                    db.Commit();
                }
            }

            return pageData;
        }
    }
}
