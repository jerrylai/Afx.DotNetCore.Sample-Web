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
    public class AfxContext : EntityContext
    {
        public bool IsBackup { get; set; } = false;
        
        public AfxContext()
            : base(new DbContextOptions<AfxContext>())
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
                    switch (m.State)
                    {
                        case EntityState.Added:
                            if (m.Entity is ICreateTime)
                            {
                                (m.Entity as ICreateTime).CreateTime = now;
                            }

                            if (m.Entity is IUpdateTime)
                            {
                                (m.Entity as IUpdateTime).UpdateTime = now;
                            }

                            if (m.Entity is IRowVersion)
                            {
                                (m.Entity as IRowVersion).RowVersion = 1;
                            }
                            break;
                        case EntityState.Modified:
                            if (m.Entity is IUpdateTime)
                            {
                                (m.Entity as IUpdateTime).UpdateTime = now;
                            }

                            if(m.Entity is IRowVersion)
                            {
                                var row = (m.Entity as IRowVersion);
                                row.RowVersion = row.RowVersion + 1;
                            }
                            break;
                        case EntityState.Deleted:
                            if (m.Entity is IIsDelete)
                            {
                                //逻辑删除
                                m.State = EntityState.Unchanged;
                                (m.Entity as IIsDelete).IsDelete = true;
                                m.Property(nameof(IIsDelete.IsDelete)).IsModified = true;

                                if (m.Entity is IUpdateTime)
                                {
                                    (m.Entity as IUpdateTime).UpdateTime = now;
                                }
                            }
                            break;
                        default:
                            break;
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
            modelBuilder.Entity<SysSequence>().HasKey(q => new { q.Name, q.Key });
            base.OnModelCreating(modelBuilder);
        }

        private static readonly Microsoft.Extensions.Logging.ILoggerFactory loggerFactory =
            new Microsoft.Extensions.Logging.LoggerFactory(new[] { new SqlLoggerProvider() });
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch(ConfigUtils.DatabaseType)
            {
                case DatabaseType.MSSQLServer:
                    optionsBuilder.UseSqlServer(ConfigUtils.ConnectionString);
                    break;
                case DatabaseType.MySQL:
                    optionsBuilder.UseMySql(ConfigUtils.ConnectionString);
                    break;
                default:
                    throw new ArgumentException("DatabaseType");
            }

            if (ConfigUtils.IsWriteSqlLog)
            {
                optionsBuilder.UseLoggerFactory(loggerFactory);
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
        /// 分布式锁
        /// </summary>
        public virtual DbSet<SysDistributedLock> SysDistributedLock { get; set; }
        /// <summary>
        /// 系统配置
        /// </summary>
        public virtual DbSet<SysConfig> SysConfig { get; set; }
        /// <summary>
        /// 系统日志
        /// </summary>
        public virtual DbSet<OpLog> OpLog { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public virtual DbSet<Region> Region { get; set; }
        /// <summary>
        /// 地区 level
        /// </summary>
        public virtual DbSet<RegionLevel> RegionLevel { get; set; }
        /// <summary>
        /// 系统队列
        /// </summary>
        public virtual DbSet<SysQueue> SysQueue { get; set; }
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
