using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.Models;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Repository
{
    public partial class SystemRepository
    {
        private void InitWebMenu(AfxContext db)
        {
            var cache = IocUtils.Get<IWebMenuCache>();
            using (db.BeginTransaction())
            {
                db.AddCommitCallback((num) => cache.Remove());
                foreach (var m in WebMenuList)
                {
                    var _m = db.WebMenu.Where(q => q.Id == m.Id).FirstOrDefault();
                    if (_m == null)
                    {
                        db.WebMenu.Add(m);
                        db.SaveChanges();
                    }
                }

                db.Commit();
            }
        }

        private static List<WebMenu> WebMenuList = new List<WebMenu>()
        {
            #region 权限管理
            new WebMenu()
            {
                Id = "1001000000000",
                Name = "权限管理",
                IsMenu = true,
                Order = 1
            },
            #region
            new WebMenu()
            {
                Id = "1001001000000",
                ParentId = "1001000000000",
                Name = "角色管理",
                IsMenu = true,
                Order = 1,
                PageUrl = "/Role/Index"
            },
            new WebMenu()
            {
                Id = "1001002000000",
                ParentId = "1001000000000",
                Name = "用户管理",
                IsMenu = true,
                Order = 2,
                PageUrl = "/User/Index"
            },
            #endregion
            #endregion
            
            #region 系统设置
            new WebMenu()
            {
                Id = "1002000000000",
                Name = "系统设置",
                IsMenu = true,
                Order = 2
            },
            #region
            new WebMenu()
            {
                Id = "1002001000000",
                ParentId = "1002000000000",
                Name = "地区管理",
                IsMenu = true,
                Order = 1,
                PageUrl = "/Region/Index"
            },
            new WebMenu()
            {
                Id = "1002002000000",
                ParentId = "1002000000000",
                Name = "系统日志",
                IsMenu = true,
                Order = 2,
                PageUrl = "/Log/Index"
            },
            #endregion
            #endregion


        };
    }
}
