using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Models;
using AfxDotNetCoreSample.Common;

namespace AfxDotNetCoreSample.Repository
{
    public class RoleWebMenuRepository : BaseRepository, IRoleWebMenuRepository
    {
        protected virtual IRoleWebMenuCache cache => this.GetCache<IRoleWebMenuCache>();

        public virtual List<string> Get(string roleId)
        {
            List<string> list = null;
            list = this.cache.Get(roleId);
            if(list == null)
            {
                using(var db = this.GetContext())
                {
                    list = db.RoleWebMenu.Where(q => q.RoleId == roleId).Select(q => q.WebMenuId).ToList();
                    this.cache.Set(roleId, list);
                }
            }

            return list;
        }

        public virtual int Update(string roleId, List<string> addWebMenuIdList, List<string> delWebMenuIdList)
        {
            int count = 0;
            using(var db = this.GetContext())
            {
                using (db.BeginTransaction())
                {
                    if (addWebMenuIdList != null && addWebMenuIdList.Count > 0)
                    {
                        var idqueue = new Queue<string>(IdGenerator.GetList<RoleWebMenu>(addWebMenuIdList.Count));
                        foreach (var webMenuId in addWebMenuIdList)
                        {
                            var m = new RoleWebMenu()
                            {
                                Id = idqueue.Dequeue(),
                                RoleId = roleId,
                                WebMenuId = webMenuId
                            };
                            db.RoleWebMenu.Add(m);
                        }
                    }
                    if (delWebMenuIdList != null && delWebMenuIdList.Count > 0)
                    {
                        foreach (var webMenuId in delWebMenuIdList)
                        {
                            var m = db.RoleWebMenu.Where(q => q.RoleId == roleId && q.WebMenuId == webMenuId).FirstOrDefault();
                            if (m != null)
                            {
                                db.RoleWebMenu.Remove(m);
                            }
                        }
                    }
                    db.AddCommitCallback((num) => this.cache.Remove(roleId));
                    count += db.SaveChanges();
                    db.Commit();
                }
            }

            return count;
        }
    }
}
