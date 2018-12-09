using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.Models;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.Common;
using System.Data;

namespace AfxDotNetCoreSample.Repository
{
    public partial class SystemRepository
    {
        private void InitRoleWebMenu(AfxContext db)
        {
            using (db.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                foreach (var kv in RoleWebMenuList)
                {
                    foreach (var webmenuId in kv.Value)
                    {
                        var m = db.RoleWebMenu.Where(q => q.RoleId == kv.Key && q.WebMenuId == webmenuId).FirstOrDefault();
                        if (m == null)
                        {
                            m = new RoleWebMenu()
                            {
                                Id = this.GetIdentity<RoleWebMenu>(),
                                RoleId = kv.Key,
                                WebMenuId = webmenuId
                            };
                            db.RoleWebMenu.Add(m);
                            db.SaveChanges();
                        }
                    }
                    db.AddCommitCallback((num) =>
                    {
                        using (var cache = IocUtils.Get<IRoleWebMenuCache>())
                        {
                            cache.Remove(kv.Key);
                        }
                    });
                }
                db.Commit();
            }
        }

        private readonly static Dictionary<string, List<string>> RoleWebMenuList = new Dictionary<string, List<string>>()
        {
            //系统管理
            {"1000", new List<string>(){ "1001000000000", "1001001000000", "1001002000000",//权限管理
            "1002000000000", "1002001000000", "1002002000000" //系统设置
            }},
            //普通管理
            {"1001", new List<string>(){ "1002000000000", "1002001000000", "1002002000000" //系统设置
            } }
        };
    }
}
