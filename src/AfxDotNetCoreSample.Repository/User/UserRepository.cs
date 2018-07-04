using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Models;

namespace AfxDotNetCoreSample.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        internal static UserRepository Instance = new UserRepository();
        
        public virtual UserDto Get(string id)
        {
            var userCache = this.GetCache<IUserCache>();
            UserDto vm = userCache.Get(id);
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

                if (vm != null) userCache.Set(id, vm);
            }

            return vm;
        }

        public virtual string GetId(string account)
        {
            var idcache = this.GetCache<IUserIdCache>();
            var id = idcache.Get(account);
            if(string.IsNullOrEmpty(id))
            {
                using (var db = this.GetContext())
                {
                    var query = from q in db.User
                                where q.Account == account && q.IsDelete == false
                                select q.Id;
                    id = query.FirstOrDefault();
                    idcache.Set(account, id);
                }
            }

            return id;
        }

        public virtual int Update(UserDto vm)
        {
            int count = 0;
            using (var db = this.GetContext())
            {
                var m = db.User.Where(q => q.Id == vm.Id && q.IsDelete == false).FirstOrDefault();
                if (m != null)
                {
                    m.Name = vm.Name;
                    if (m.IsSystem == false)
                    {
                        m.RoleId = vm.RoleId;
                    }
                    m.Mobile = vm.Mobile;
                    m.Mail = vm.Mail;
                    m.Status = vm.Status;
                    if (!string.IsNullOrEmpty(vm.Password))
                    {
                        m.Password = vm.Password;
                    }
                    count = db.SaveChanges();
                    vm.UpdateTime = m.UpdateTime;
                    var userCache = this.GetCache<IUserCache>();
                    userCache.Remove(m.Id);
                }
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
                    count = db.SaveChanges();
                    var userCache = this.GetCache<IUserCache>();
                    userCache.Remove(m.Id);
                }
            }

            return count;
        }

        public virtual int Add(UserDto vm)
        {
            int count = 0;
            using (var db = this.GetContext())
            {
                vm.Account = vm.Account.ToLower();
                var user = db.User.Where(q => q.Account == vm.Account).FirstOrDefault();
                if (user == null)
                {
                    var m = new User()
                    {
                        Id = IdGenerator.Get<User>(),
                        Account = vm.Account,
                        Name = vm.Name,
                        RoleId = vm.RoleId,
                        Mail = vm.Mail,
                        Mobile = vm.Mobile,
                        Status = vm.Status,
                        Password = string.IsNullOrEmpty(vm.Password) ? null : vm.Password,
                        IsSystem = false,
                        IsDelete = false
                    };

                    count += db.SaveChanges();
                    vm.Id = m.Id;
                    vm.UpdateTime = m.UpdateTime;
                    vm.CreateTime = m.CreateTime;
                    var idcache = this.GetCache<IUserIdCache>();
                    idcache.Remove(vm.Account);
                }
            }

            return count;
        }

        public virtual int Delete(string id)
        {
            int count = 0;
            using (var db = this.GetContext())
            {
                var m = db.User.Where(q => q.Id == id && q.IsSystem == false && q.IsDelete == false).FirstOrDefault();
                if (m != null)
                {
                    db.User.Remove(m);
                    count += db.SaveChanges();
                    var userCache = this.GetCache<IUserCache>();
                    userCache.Remove(m.Id);
                    var idcache = this.GetCache<IUserIdCache>();
                    idcache.Remove(m.Account);
                }
            }
            return count;
        }

    }
}
