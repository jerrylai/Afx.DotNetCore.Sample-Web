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
        private void InitRole(AfxContext db)
        {
            using (db.BeginTransaction())
            {
                foreach (var m in roles)
                {
                    var vm = db.Role.Where(q => q.Id == m.Id).FirstOrDefault();
                    if (vm == null)
                    {
                        db.Role.Add(m);
                        db.SaveChanges();
                    }
                }

                db.Commit();
            }
        }

        private readonly static List<Role> roles = new List<Role>()
        {
            new Role()
            {
                Id = "1000",
                Name = "系统管理",
                IsSystem = true,
                IsDelete = false
            },
            new Role()
            {
                Id = "1001",
                Name = "普通管理",
                IsSystem = true,
                IsDelete = false
            }
        };
    }
}
