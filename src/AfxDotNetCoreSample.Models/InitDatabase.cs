using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afx.Data.Entity.Schema;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Models
{
    public class InitDatabase
    {
        internal static void WriteSQL(string sql)
        {
            if (ConfigUtils.IsWriteSqlLog)
            {
               LogUtils.Debug("【SQL】" + sql);
            }

        }
        
        static object lockObj = new object();
        static bool IsInit = false;
        public static void InitDb(AfxDotNetCoreSampleContext db)
        {
            if (!ConfigUtils.InitDatabase || IsInit) return;
            lock (lockObj)
            {
                if (IsInit) return;
                string connectionString = ConfigUtils.ConnectionString;
                IDatabaseSchema databaseSchema = null;
                ITableSchema tableSchema = null;
                switch (ConfigUtils.DatabaseType)
                {
                    case DatabaseType.MSSQLServer:
                        databaseSchema = new Afx.Data.MSSQLServer.Entity.Schema.MsSqlDatabaseSchema(connectionString);
                        tableSchema = new Afx.Data.MSSQLServer.Entity.Schema.MsSqlTableSchema(connectionString);
                        break;
                    case DatabaseType.MySQL:
                        databaseSchema = new Afx.Data.MySql.Entity.Schema.MySqlDatabaseSchema(connectionString);
                        tableSchema = new Afx.Data.MySql.Entity.Schema.MySqlTableSchema(connectionString);
                        break;
                    default:
                        throw new Exception("【InitDatabase】InitDb, 数据库类型错误！");
                }
                //更新数据库结构sql 日志
                databaseSchema.Log = WriteSQL;
                tableSchema.Log = WriteSQL;
                using (var build = new BuildDatabase(databaseSchema, tableSchema))
                {
                    build.Build<AfxDotNetCoreSampleContext>();
                }

                IsInit = true;
            }
        }
    }
}
