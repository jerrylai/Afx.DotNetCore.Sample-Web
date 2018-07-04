using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Models;

namespace AfxDotNetCoreSample.Repository
{
    public class RoleWebMenuRepository : BaseRepository, IRoleWebMenuRepository
    {
        internal static RoleWebMenuRepository Instance = new RoleWebMenuRepository();

        public virtual List<string> Get(string roleId)
        {
            List<string> list = null;
            var roleWebMenuCache = this.GetCache<IRoleWebMenuCache>();
            list = roleWebMenuCache.Get(roleId);
            if(list == null)
            {
                using(var db = this.GetContext())
                {
                    list = db.RoleWebMenu.Where(q => q.RoleId == roleId).Select(q => q.WebMenuId).ToList();
                    roleWebMenuCache.Set(roleId, list);
                }
            }

            return list;
        }




        public virtual int Update(string roleId, List<string> list)
        {
            int count = 0;
            using(var db = this.GetContext())
            {
                using (db.BeginTransaction())
                {
                    var old = db.RoleWebMenu.Where(q => q.RoleId == roleId).ToList();
                    var del = old.Where(q => !list.Contains(q.WebMenuId)).ToList();
                    var add = list.Where(q => old.FirstOrDefault(m => m.WebMenuId == q) == null).ToList();
                    del.ForEach(m => { db.RoleWebMenu.Remove(m); });
                    count += db.SaveChanges();
                    foreach(var item in add)
                    {
                        var m = new RoleWebMenu()
                        {
                            Id = IdGenerator.Get<RoleWebMenu>(),
                            RoleId = roleId,
                            WebMenuId = item
                        };
                        db.RoleWebMenu.Add(m);
                    }
                    count += db.SaveChanges();
                    db.Commit();
                    if (count > 0)
                    {
                        var cache = this.GetCache<IRoleWebMenuCache>();
                        cache.Remove(roleId);
                    }
                }
            }

            return count;
        }
    }
}
