using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Afx.Data.Entity;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.Common;
using System.Threading;

namespace AfxDotNetCoreSample.Models
{
    public class AfxDotNetCoreSampleContext : EntityContext
    {
        public bool IsBackup { get; set; } = false;
        
        public AfxDotNetCoreSampleContext()
            : base(new DbContextOptions<AfxDotNetCoreSampleContext>())
        {
            
        }
        
        /// <summary>
        /// 转义关键字列名、表名
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public virtual string GetColumn(string column)
        {
            switch (ConfigUtils.DatabaseType)
            {
                case DatabaseType.MSSQLServer:
                    return string.Format("[{0}]", column);
                case DatabaseType.MySQL:
                    return string.Format("`{0}`", column);
                default:
                    throw new ArgumentException("DatabaseType");
            }
        }

        /// <summary>
        /// 获取参数名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string GetParamterName(string name)
        {
            switch (ConfigUtils.DatabaseType)
            {
                case DatabaseType.MSSQLServer:
                    return string.Format("@{0}", name);
                case DatabaseType.MySQL:
                    return string.Format("?{0}", name);
                default:
                    throw new ArgumentException("DatabaseType");
            }
        }

        /// <summary>
        /// 获取参数名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual System.Data.Common.DbParameter GetParamter(string name, object value)
        {
            switch (ConfigUtils.DatabaseType)
            {
                case DatabaseType.MSSQLServer:
                    return new global::System.Data.SqlClient.SqlParameter("@" + name, value);
                case DatabaseType.MySQL:
                    return new global::MySql.Data.MySqlClient.MySqlParameter("?" + name, value);
                default:
                    throw new ArgumentException("DatabaseType");
            }
        }
        
        /// <summary>
        /// 获取数据库utc时间
        /// </summary>
        /// <returns></returns>
        public virtual DateTime GetUtcNow()
        {
            var now = DateTime.Now.ToUniversalTime();
            Afx.Data.Database db = null;
            switch (ConfigUtils.DatabaseType)
            {
                case DatabaseType.MSSQLServer:
                    db = new Afx.Data.MSSQLServer.MsSqlDatabase(ConfigUtils.ConnectionString);
                    break;
                case DatabaseType.MySQL:
                    db = new Afx.Data.MySql.MySqlDatabase(ConfigUtils.ConnectionString);
                    break;
                default:
                    throw new ArgumentException("DatabaseType");
            }
            using (db)
            {
                now = db.GetUtcNow();
            }

            return now;
        }

        /// <summary>
        /// 获取数据库本地时间
        /// </summary>
        /// <returns></returns>
        public virtual DateTime GetLocalNow()
        {
            var now = this.GetUtcNow();
            now = now.ToLocalTime();

            return now;
        }
        
        private void SetPropertyDefaultValue()
        {
            if (!this.IsBackup)
            {
                var now = this.GetLocalNow();
                foreach (var m in this.ChangeTracker.Entries())
                {
                    if (m.State == EntityState.Added && m.Entity is ICreateTime)
                    {
                        m.Property(nameof(ICreateTime.CreateTime)).CurrentValue = now;
                    }
                    else if (m.State == EntityState.Deleted && m.Entity is IIsDelete)
                    {
                        //逻辑删除
                        m.State = EntityState.Unchanged;
                        var p = m.Property(nameof(IIsDelete.IsDelete));
                        p.CurrentValue = true;
                        p.IsModified = true;
                    }

                    if ((m.State == EntityState.Added || m.State == EntityState.Modified)
                        && m.Entity is IUpdateTime)
                    {
                        m.Property(nameof(IUpdateTime.UpdateTime)).CurrentValue = now;
                    }
                }
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.SetPropertyDefaultValue();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.SetPropertyDefaultValue();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch(ConfigUtils.DatabaseType)
            {
                case DatabaseType.MSSQLServer:
                    optionsBuilder.UseSqlServer(ConfigUtils.ConnectionString);
                    break;
                case DatabaseType.MySQL:
                    optionsBuilder.UseMySQL(ConfigUtils.ConnectionString);
                    break;
                default:
                    throw new ArgumentException("DatabaseType");
            }
            base.OnConfiguring(optionsBuilder);
        }

        public override void Dispose()
        {
            this.ClearCommitCallback();
            base.Dispose();
        }

        #region Models
        #region sys
        /// <summary>
        /// 序列号
        /// </summary>
        public virtual DbSet<SysSequence> SysSequence { get; set; }
        /// <summary>
        /// 执行状态锁
        /// </summary>
        public virtual DbSet<SysTaskLock> SysTaskLock { get; set; }
        /// <summary>
        /// 系统配置
        /// </summary>
        public virtual DbSet<SysConfig> SysConfig { get; set; }
        #endregion

        #region Common
        /// <summary>
        /// 系统日志
        /// </summary>
        public virtual DbSet<OpLog> OpLog { get; set; }
        #endregion

        #region user
        /// <summary>
        /// 角色
        /// </summary>
        public virtual DbSet<Role> Role { get; set; }
        /// <summary>
        /// WEB菜单
        /// </summary>
        public virtual DbSet<WebMenu> WebMenu { get; set; }
        /// <summary>
        /// 角色WEB菜单关系
        /// </summary>
        public virtual DbSet<RoleWebMenu> RoleWebMenu { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public virtual DbSet<User> User { get; set; }
        #endregion
        
        #endregion
    }
}
