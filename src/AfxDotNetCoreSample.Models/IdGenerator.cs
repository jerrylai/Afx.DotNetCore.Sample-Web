using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using Afx.Data;
using Afx.Threading;
using System.Linq;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.Common;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// id 生成器
    /// </summary>
    public static class IdGenerator
    {
        class IdInfoModel
        {
            public string Key { get; set; }

            public object LockObj { get; private set; }

            public int StratValue { get; set; }

            public int EndValue { get; set; }

            public IdInfoModel()
            {
                this.LockObj = new object();
            }
        }

        public static int CacheCount => ConfigUtils.IdGeneratorCount;
        public static string ServerId => ConfigUtils.IdGeneratorServerId;

        public static int FormatNum => ConfigUtils.IdGeneratorFormatNum;

        private static ReadWriteLock rwLock = new ReadWriteLock();
        private static Dictionary<string, IdInfoModel> dic = new Dictionary<string, IdInfoModel>(StringComparer.OrdinalIgnoreCase);


        private static IDatabase GetDb()
        {
            if(string.IsNullOrEmpty(ConfigUtils.ConnectionString))
            {
                throw new Exception("【IdGenerator】ConnectionString 不能为空！");
            }
            IDatabase db = null;
            switch (ConfigUtils.DatabaseType)
            {
                case DatabaseType.MSSQLServer:
                    db = new Afx.Data.MSSQLServer.MsSqlDatabase(ConfigUtils.ConnectionString);
                    break;
                case DatabaseType.MySQL:
                    db = new Afx.Data.MySql.MySqlDatabase(ConfigUtils.ConnectionString);
                    break;
                default:
                    throw new Exception($"【IdGenerator】不支持 {ConfigUtils.DatabaseType} 数据库！");
            }

            return db;
        }

        private static int GetDbValue(string name, string key, int count)
        {
            int value = 0;

            using (var db = GetDb())
            {
                using (db.BeginTransaction())
                {
                    StringBuilder updateSql = new StringBuilder();
                    updateSql.Append("update ");
                    updateSql.Append(db.EncodeColumn("SysSequence"));
                    updateSql.Append(" set ");
                    updateSql.Append(db.EncodeColumn("Value"));
                    updateSql.Append(" = ");
                    updateSql.Append(db.EncodeColumn("Value"));
                    updateSql.Append(" + @Value, ");
                    updateSql.Append(db.EncodeColumn("UpdateTime"));
                    updateSql.Append(" = @UpdateTime  where ");
                    updateSql.Append(db.EncodeColumn("Name"));
                    updateSql.Append(" = @Name and ");
                    updateSql.Append(db.EncodeColumn("Key"));
                    updateSql.Append(" = @Key");
                    var now = DateTime.Now;
                    var m = new SysSequence { Value = count, UpdateTime = now, Name = name, Key = key };
                    int c = db.ExecuteNonQuery(updateSql.ToString(), m);
                    if (c == 0)
                    {
                        StringBuilder insertSql = new StringBuilder();
                        insertSql.Append("insert into ");
                        insertSql.Append(db.EncodeColumn("SysSequence"));
                        insertSql.Append("(");
                        insertSql.Append(db.EncodeColumn("Id"));
                        insertSql.Append(", ");
                        insertSql.Append(db.EncodeColumn("Name"));
                        insertSql.Append(", ");
                        insertSql.Append(db.EncodeColumn("Key"));
                        insertSql.Append(", ");
                        insertSql.Append(db.EncodeColumn("Value"));
                        insertSql.Append(", ");
                        insertSql.Append(db.EncodeColumn("UpdateTime"));
                        insertSql.Append(", ");
                        insertSql.Append(db.EncodeColumn("CreateTime"));
                        insertSql.Append(") values(@Id, @Name, @Key, @Value, @UpdateTime, @CreateTime)");

                        m.Id = Guid.NewGuid().ToString("n");
                        m.CreateTime = now;
                        try
                        {
                            c = db.ExecuteNonQuery(insertSql.ToString(), m);
                        }
                        catch
                        {
                            c = db.ExecuteNonQuery(updateSql.ToString(), m);
                        }
                    }

                    StringBuilder selectSql = new StringBuilder();
                    selectSql.Append("select ");
                    selectSql.Append(db.EncodeColumn("Value"));
                    selectSql.Append(" from ");
                    selectSql.Append(db.EncodeColumn("SysSequence"));
                    selectSql.Append(" where ");
                    selectSql.Append(db.EncodeColumn("Name"));
                    selectSql.Append(" = @Name and ");
                    selectSql.Append(db.EncodeColumn("Key"));
                    selectSql.Append(" = @Key");
                    value = db.ExecuteScalar<int>(selectSql.ToString(), m);

                    db.Commit();
                }
            }

            return value;
        }

        private static int GetValue(string name, string key, int count)
        {
            int value = 0;
            IdInfoModel vm = null;
            using (rwLock.GetReadLock())
            {
                dic.TryGetValue(name, out vm);
            }

            if (vm == null)
            {
                using (rwLock.GetWriteLock())
                {
                    dic.TryGetValue(name, out vm);
                    if (vm == null)
                    {
                        vm = new IdInfoModel()
                        {
                            Key = string.Empty,
                            StratValue = 0,
                            EndValue = 0
                        };
                        dic.Add(name, vm);
                    }
                }
            }
            
            lock (vm.LockObj)
            {
                if (string.Equals(vm.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    int c = vm.EndValue - vm.StratValue;
                    if (c < count)
                    {
                        vm.EndValue = GetDbValue(name, key, CacheCount + count - c);
                    }
                }
                else
                {
                    vm.EndValue = GetDbValue(name, key, CacheCount + count);
                    vm.StratValue = vm.EndValue - CacheCount - count;
                    vm.Key = key;
                }
                vm.StratValue = vm.StratValue + count;
                value = vm.StratValue;
            }

            return value;
        }

        private static string GetName(Type type)
        {
            return type.Name;
        }

        private static string GetKey(string name)
        {
            string key = $"{DateTime.Now.ToString("yyyyMMdd")}{ServerId}";

            return key;
        }

        private static string FormatId(string key, int value)
        {
            string format = "{0}{1:D" + FormatNum + "}";
            string id = string.Format(format, key, value);

            return id;
        }

        /// <summary>
        /// 获取id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string Get<T>() where T : class, IModel
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// 获取id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string Get(Type type)
        {
            string id = null;
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!typeof(IModel).IsAssignableFrom(type)) throw new ArgumentException($"{type.FullName} is not IModel!");

            string name = GetName(type);
            string key = GetKey(name);
            int value = GetValue(name, key, 1);
            id = FormatId(key, value);

            return id;
        }

        /// <summary>
        /// 批量获取id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count">获取个数</param>
        /// <returns></returns>
        public static List<string> GetList<T>(int count) where T : class, IModel
        {
            if (count <= 0) count = 0;
            List<string> list = null;
            if (count > 0)
            {
                var t = typeof(T);
                string name = GetName(t);
                string key = GetKey(name);
                int value = GetValue(name, key, count);
                list = new List<string>(count);
                for (int i = value - count + 1; i <= value; i++)
                {
                    string id = FormatId(key, i);
                    list.Add(id);
                }
            }
            else
            {
                list = new List<string>(0);
            }

            return list;
        }
    }
}
