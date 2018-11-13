using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.Models;

namespace AfxDotNetCoreSample.Repository
{
    public partial class SystemRepository
    {
        private void InitUser(AfxContext db)
        {
            using (db.BeginTransaction())
            {
                var m = db.User.Where(q => q.Account == "admin").FirstOrDefault();
                if (m == null)
                {
                    var role = db.Role.Where(q => q.Name == "系统管理" && q.IsSystem == true).FirstOrDefault();
                    m = new User()
                    {
                        Id = "1000",
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
    }
}
