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

namespace AfxDotNetCoreSample.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        protected virtual IUserCache userCache => this.GetCache<IUserCache>();

        protected virtual IUserIdCache userIdCache => this.GetCache<IUserIdCache>();

        public virtual UserDto Get(string id)
        {
            UserDto vm = this.userCache.Get(id);
            if(vm == null)
            {
                using (var db = this.GetContext())
                {
                    var query = from q in db.User
                                where q.Id == id && q.IsDelete == false
                                select new UserDto
                                {
                                    Id = q.Id,
                                    Account = q.Account,
                                    Name = q.Name,
                                    Password = q.Password,
                                    RoleId = q.RoleId,
                                    Mail = q.Mail,
                                    Mobile = q.Mobile,
                                    Status = q.Status,
                                    IsSystem = q.IsSystem,
                                    UpdateTime = q.UpdateTime,
                                    CreateTime = q.CreateTime
                                };
                    vm = query.FirstOrDefault();
                }

                if (vm != null) this.userCache.Set(id, vm);
            }

            return vm;
        }

        public virtual string GetId(string account)
        {
            if (string.IsNullOrEmpty(account)) return null;
            account = account.ToLower();
            var id = this.userIdCache.Get(UserIdCacheType.Account,account);
            if(string.IsNullOrEmpty(id))
            {
                using (var db = this.GetContext())
                {
                    var query = from q in db.User
                                where q.Account == account && q.IsDelete == false
                                select q.Id;
                    id = query.FirstOrDefault();
                    this.userIdCache.Set(UserIdCacheType.Account, account, id);
                }
            }

            return id;
        }

        public virtual string GetIdByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return null;
            var id = this.userIdCache.Get(UserIdCacheType.Mobile, mobile);
            if (string.IsNullOrEmpty(id))
            {
                using (var db = this.GetContext())
                {
                    var query = from q in db.User
                                where q.Mobile == mobile && q.IsDelete == false
                                select q.Id;
                    id = query.FirstOrDefault();
                    this.userIdCache.Set(UserIdCacheType.Mobile, mobile, id);
                }
            }

            return id;
        }

        public virtual string GetIdByMail(string mail)
        {
            if (string.IsNullOrEmpty(mail)) return null;
            mail = mail.ToLower();
            var id = this.userIdCache.Get(UserIdCacheType.Mail, mail);
            if (string.IsNullOrEmpty(id))
            {
                using (var db = this.GetContext())
                {
                    var query = from q in db.User
                                where q.Mail == mail && q.IsDelete == false
                                select q.Id;
                    id = query.FirstOrDefault();
                    this.userIdCache.Set(UserIdCacheType.Mail, mail, id);
                }
            }

            return id;
        }

        public virtual int Update(UserDto vm)
        {
            using (var db = this.GetContext())
            {
                return this.Update(vm, db);
            }
        }

        public virtual int Update(UserDto vm, AfxContext db)
        {
            int count = 0;
            var m = db.User.Where(q => q.Id == vm.Id && q.IsDelete == false).FirstOrDefault();
            if (m != null)
            {
                m.Name = vm.Name;
                m.RoleId = vm.RoleId;
                m.Status = vm.Status;
                var oldMobile = m.Mobile;
                var oldMail = m.Mail;
                m.Mobile = vm.Mobile;
                m.Mail = vm.Mail;
                if (!string.IsNullOrEmpty(vm.Password))
                {
                    m.Password = vm.Password;
                }
                db.AddCommitCallback((num) =>
                {
                    this.userCache.Remove(m.Id);
                    if (!string.IsNullOrEmpty(oldMail) && m.Mail != oldMail) this.userIdCache.Remove(UserIdCacheType.Mail, oldMail);
                    if (!string.IsNullOrEmpty(oldMobile) && m.Mobile != oldMobile) this.userIdCache.Remove(UserIdCacheType.Mobile, oldMobile);
                });
                count = db.SaveChanges();
                vm.UpdateTime = m.UpdateTime;
            }

            return count;
        }

        public virtual int UpdatePassword(string id, string pwd)
        {
            int count = 0;
            using(var db = this.GetContext())
            {
                var m = db.User.Where(q => q.Id == id && q.IsDelete == false).FirstOrDefault();
                if(m != null)
                {
                    m.Password = pwd;
                    db.AddCommitCallback((num) => this.userCache.Remove(m.Id));
                    count = db.SaveChanges();
                }
            }

            return count;
        }

        public virtual int Add(UserDto vm)
        {
            using (var db = this.GetContext())
            {
                using (db.BeginTransaction())
                {
                    int count = this.Add(vm, db);
                    db.Commit();
                    return count;
                }
            }
        }

        public virtual int Add(UserDto vm, AfxContext db)
        {
            int count = 0;
            var m = new User()
            {
                Id = IdGenerator.Get<User>(),
                Account = vm.Account.ToLower(),
                Name = vm.Name,
                RoleId = vm.RoleId,
                Mail = vm.Mail?.ToLower(),
                Mobile = vm.Mobile,
                Status = vm.Status,
                Password = !string.IsNullOrEmpty(vm.Password) ? vm.Password : null,
                IsSystem = false,
                IsDelete = false
            };

            db.AddCommitCallback((num) =>
            {
                this.userIdCache.Remove(UserIdCacheType.Account, m.Account);
                if (!string.IsNullOrEmpty(m.Mail)) this.userIdCache.Remove(UserIdCacheType.Mail, m.Mail);
                if (!string.IsNullOrEmpty(m.Mobile)) this.userIdCache.Remove(UserIdCacheType.Mobile, m.Mobile);
            });
            db.Add(m);
            db.SaveChanges();
            vm.Id = m.Id;
            vm.UpdateTime = m.UpdateTime;
            vm.CreateTime = m.CreateTime;

            return count;
        }

        public virtual int Delete(string id)
        {
            int count = 0;
            using (var db = this.GetContext())
            {
                count = this.Delete(id, db);
            }
            return count;
        }

        public virtual int Delete(string id, AfxContext db)
        {
            int count = 0;

            var m = db.User.Where(q => q.Id == id && q.IsSystem == false && q.IsDelete == false).FirstOrDefault();
            if (m != null)
            {
                db.User.Remove(m);
                db.AddCommitCallback((num) =>
                {
                    this.userCache.Remove(m.Id);
                    this.userIdCache.Remove(UserIdCacheType.Account, m.Account);
                    if (!string.IsNullOrEmpty(m.Mail)) this.userIdCache.Remove(UserIdCacheType.Mail, m.Mail);
                    if (!string.IsNullOrEmpty(m.Mobile)) this.userIdCache.Remove(UserIdCacheType.Mobile, m.Mobile);
                });
                count += db.SaveChanges();
            }

            return count;
        }

        public virtual PageDataOutputDto<UserDto> GetPageData(UserPageInputDto vm)
        {
            PageDataOutputDto<UserDto> pageData = null;
            using (var db = this.GetContext())
            {
                var query = from q in db.User
                            where q.IsDelete == false
                            select new UserDto
                            {
                                Id = q.Id,
                                Account = q.Account,
                                Name = q.Name,
                                //Password = q.Password,
                                RoleId = q.RoleId,
                                Mail = q.Mail,
                                Mobile = q.Mobile,
                                Status = q.Status,
                                IsSystem = q.IsSystem,
                                UpdateTime = q.UpdateTime,
                                CreateTime = q.CreateTime
                            };

                if (!string.IsNullOrEmpty(vm.RoleId))
                {
                    query = query.Where(q => q.RoleId == vm.RoleId);
                }

                if (vm.Status.HasValue)
                {
                    var status = vm.Status.Value;
                    query = query.Where(q => q.Status == status);
                }

                if (!string.IsNullOrEmpty(vm.Keyword))
                {
                    var value = vm.Keyword.DbLike(DbLikeType.All);
                    query = query.Where(q => EF.Functions.Like(q.Account, value) || EF.Functions.Like(q.Name, value) || EF.Functions.Like(q.Mobile, value) || EF.Functions.Like(q.Mail, value));
                }

                pageData = query.ToPage(vm);
            }

            return pageData;
        }

        public virtual int GetUserCount(string roleId)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(roleId))
            {
                using(var db = this.GetContext())
                {
                    count = db.User.Where(q => q.RoleId == roleId && q.IsDelete == false).Count();
                }
            }

            return count;
        }
    }
}
