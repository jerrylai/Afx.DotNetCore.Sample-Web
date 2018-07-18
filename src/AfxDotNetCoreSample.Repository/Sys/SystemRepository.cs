using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.Models;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.IRepository;

namespace AfxDotNetCoreSample.Repository
{
    /// <summary>
    /// 系统相关存储实现
    /// </summary>
    public class SystemRepository : BaseRepository, ISystemRepository
    {
        internal readonly static SystemRepository Instance = new SystemRepository();

        /// <summary>
        /// 更新数据库结构
        /// </summary>
        public virtual void InitDb()
        {
            InitDatabase.InitDb(this.GetContext());
        }

        /// <summary>
        /// 初始化系统数据
        /// </summary>
        /// <returns></returns>
        public virtual void InitData()
        {
            using (var db = this.GetContext())
            {
                this.InitRole(db);
                this.InitUser(db);
                //this.InitWebMenu(db);
                //this.InitRoleWebMenu(db);
            }
        }
        
        private void InitRole(AfxDotNetCoreSampleContext db)
        {
            using (db.BeginTransaction())
            {
                var m = db.Role.Where(q => q.Type == RoleType.Admin && q.IsSystem == true).FirstOrDefault();
                if (m == null)
                {
                    m = new Role()
                    {
                        Id = "10000000000000000000",
                        Type = RoleType.Admin,
                        Name = "系统管理",
                        IsSystem = true,
                        IsDelete = false
                    };
                    db.Role.Add(m);
                    db.SaveChanges();
                }
                
                m = db.Role.Where(q => q.Type == RoleType.User && q.IsSystem == true).FirstOrDefault();
                if (m == null)
                {
                    m = new Role()
                    {
                        Id = "10000000000000000001",
                        Type = RoleType.User,
                        Name = "普通用户",
                        IsSystem = true,
                        IsDelete = false
                    };
                    db.Role.Add(m);
                    db.SaveChanges();
                }

                db.Commit();
            }
        }
        
        private void InitUser(AfxDotNetCoreSampleContext db)
        {
            using (db.BeginTransaction())
            {
                var m = db.User.Where(q => q.Account == "admin").FirstOrDefault();
                if (m == null)
                {
                    var role = db.Role.Where(q => q.Type == RoleType.Admin && q.IsSystem == true).FirstOrDefault();
                    m = new User()
                    {
                        Id = "10000000000000000000",
                        RoleId = role.Id,
                        Name = "管理员",
                        Account = "admin",
                        Status = UserStatus.Enabled,
                        IsSystem = true,
                        IsDelete = false
                    };
                    db.User.Add(m);
                    db.SaveChanges();
                }

                db.Commit();
            }
        }

        private void InitWebMenu(AfxDotNetCoreSampleContext db)
        {
            using (db.BeginTransaction())
            {
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

        private void InitRoleWebMenu(AfxDotNetCoreSampleContext db)
        {
            using (db.BeginTransaction())
            {
                var role = db.Role.Where(q => q.Type == RoleType.Admin && q.IsSystem == true).FirstOrDefault();
                foreach (var webMenu in WebMenuList)
                {
                    if (webMenu.Id == "400000000" || webMenu.ParentId == "400000000") continue;
                    var m = db.RoleWebMenu.Where(q => q.RoleId == role.Id && q.WebMenuId == webMenu.Id).FirstOrDefault();
                    if (m == null)
                    {
                        m = new RoleWebMenu()
                        {
                            Id = IdGenerator.Get<RoleWebMenu>(),
                            RoleId = role.Id,
                            WebMenuId = webMenu.Id
                        };
                        db.RoleWebMenu.Add(m);
                        db.SaveChanges();
                    }
                }
                
                role = db.Role.Where(q => q.Type == RoleType.User && q.IsSystem == true).FirstOrDefault();
                foreach (var id in UserRoleWebMenuList)
                {
                    var m = db.RoleWebMenu.Where(q => q.RoleId == role.Id && q.WebMenuId == id).FirstOrDefault();
                    if (m == null)
                    {
                        m = new RoleWebMenu()
                        {
                            Id = IdGenerator.Get<RoleWebMenu>(),
                            RoleId = role.Id,
                            WebMenuId = id
                        };
                        db.RoleWebMenu.Add(m);
                        db.SaveChanges();
                    }
                }

                db.Commit();
            }
        }


        private static List<string> UserRoleWebMenuList = new List<string>()
        {
            "100000000",
            "400000000","400100000","400200000","400300000","400400000",
             "700000000","700100000",
        };

        private static List<WebMenu> WebMenuList = new List<WebMenu>()
        {
             #region 组织架构
            new WebMenu()
            {
                Id = "100000000",
                IsMenu = true,
                Name = "组织架构",
                Order = 10000,
                Url = "/html/OrgList.html"
            },
            #endregion

            #region 用户管理
            new WebMenu()
            {
                Id = "200000000",
                IsMenu = true,
                Name = "系统权限",
                Order = 10000
            },
             new WebMenu()
            {
                Id = "200100000",
                ParentId = "200000000",
                IsMenu = true,
                Name = "角色管理",
                Order = 10000,
                Url = "/html/User/RoleList.html"
            },
            new WebMenu()
            {
                Id = "200200000",
                ParentId = "200000000",
                IsMenu = true,
                Name = "用户管理",
                Order = 10000,
                Url = "/html/User/UserList.html"
            },
            #endregion

        };
    }
}
