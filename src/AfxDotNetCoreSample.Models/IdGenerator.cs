using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using Afx.Data;
using Afx.Threading;
using System.Linq;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Models
{
    /// <summary>
    /// 分布式id生成器接口 add by jerrylai@liyun.com
    /// </summary>
    public interface IIdGenerator : IDisposable
    {
        DatabaseType DatabaseType { get; }
        string ConnectionString { get; }
        int CacheCount { get; }
        string ServerId { get; }
        int FormatNum { get; }
        bool IsDisposed { get; }

        string Get<T>(bool withDate = true, bool withServerId = true, int? formatNum = null, int? cacheCount = null) where T : class, IModel;

        string Get(Type type, bool withDate = true, bool isServerId = true, int? formatNum = null, int? cacheCount = null);

        List<string> GetList<T>(int count, bool withDate = true, bool isServerId = true, int? formatNum = null, int? cacheCount = null) where T : class, IModel;

        List<string> GetList(Type type, int count, bool withDate = true, bool isServerId = true, int? formatNum = null, int? cacheCount = null);
    }

    /// <summary>
    /// 分布式id生成器 add by jerrylai@liyun.com
    /// </summary>
    public sealed class IdGenerator : IIdGenerator, IDisposable
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

        public DatabaseType DatabaseType { get; private set; }
        public string ConnectionString { get; private set; }
        public int CacheCount { get; private set; }
        public string ServerId { get; private set; }
        public int FormatNum { get; private set; }
        public bool IsDisposed { get; private set; }

        private object dicLock;
        private Dictionary<string, IdInfoModel> dic;
        private IdInfoModel sq;
        private readonly string updateSQL;
        private readonly string selectUpSQL;
        private readonly string insertSQL;

        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.IsDisposed = true;
                this.ConnectionString = null;
                this.ServerId = null;
                if (this.dic != null) this.dic.Clear();
                this.dic = null;
                this.dicLock = null;
                this.sq = null;
            }
        }

        public IdGenerator(DatabaseType type, string connectionString, string serverId = "101", int cacheCount = 100, int formatNum = 8)
        {
            this.IsDisposed = true;
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            if (string.IsNullOrEmpty(serverId)) throw new ArgumentNullException(nameof(connectionString));
            if (0 >= cacheCount) throw new ArgumentException($"{nameof(cacheCount)} is error!", nameof(cacheCount));
            if (0 > formatNum) throw new ArgumentException($"{nameof(formatNum)} is error!", nameof(formatNum));
            this.IsDisposed = false;
            this.dicLock = new object();
            this.dic = new Dictionary<string, IdInfoModel>(StringComparer.OrdinalIgnoreCase);
            this.DatabaseType = type;
            this.ConnectionString = connectionString;
            this.ServerId = serverId;
            this.CacheCount = cacheCount;
            this.FormatNum = formatNum;
            this.sq = new IdInfoModel() { Key = "*", StratValue = 0, EndValue = 0 };

            switch (type)
            {
                case DatabaseType.MSSQLServer:
                    updateSQL = "UPDATE [SysSequence] SET [Value] = [Value] + @count, [UpdateTime] = @now WHERE [Name] = @name AND [Key] = @key";
                    selectUpSQL = "SELECT [Value] FROM [SysSequence] WHERE [Name] = @name AND [Key] = @key";

                    insertSQL = "INSERT INTO [SysSequence]([Name], [Key], [Value], [UpdateTime], [CreateTime]) VALUES(@Name, @Key, @Value, @UpdateTime, @CreateTime)";
                    break;
                case DatabaseType.MySQL:
                    updateSQL = "UPDATE `SysSequence` SET `Value` = `Value` + @count, `UpdateTime` = @now WHERE `Name` = @name AND `Key` = @key";
                    selectUpSQL = "SELECT `Value` FROM `SysSequence` WHERE `Name` = @name AND `Key` = @key";

                    insertSQL = "INSERT INTO `SysSequence`(`Name`, `Key`, `Value`, `UpdateTime`, `CreateTime`) VALUES(@Name, @Key, @Value, @UpdateTime, @CreateTime)";
                    break;
                default:
                    throw new Exception($"不支持 {this.DatabaseType} 数据库！");
            }
        }

        private IDatabase GetDb()
        {
            switch (this.DatabaseType)
            {
                case DatabaseType.MSSQLServer:
                    return new Afx.Data.MSSQLServer.MsSqlDatabase(this.ConnectionString);
                case DatabaseType.MySQL:
                    return new Afx.Data.MySql.MySqlDatabase(this.ConnectionString);
                default:
                    throw new Exception($"不支持 {this.DatabaseType} 数据库！");
            }
        }

        private int UpdateValue(string name, string key, int count, IDatabase db)
        {
            int value = -1;
            using (db.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                int c = db.ExecuteNonQuery(this.updateSQL, new { count, now = DateTime.Now, name, key });
                if (c > 0)
                {
                    value = db.ExecuteScalar<int>(selectUpSQL, new { name, key });
                }
                db.Commit();
            }

            return value;
        }

        private bool InsertVaule(string name, string key, int count, IDatabase db)
        {
            bool result = false;
            var now = DateTime.Now;
            var m = new SysSequence { Name = name, Key = key, Value = count, UpdateTime = now, CreateTime = now };
            try
            {
                var c = db.ExecuteNonQuery(this.insertSQL, m);
                result = true;
            }
            catch { }

            return result;
        }

        private int GetDbValue(string name, string key, int count)
        {
            int value = -1;

            using (var db = GetDb())
            {
                value = this.UpdateValue(name, key, count, db);
                if (value <= 0 && this.InsertVaule(name, key, count, db))
                {
                    value = count;
                }
                else if (value <= 0)
                {
                    value = this.UpdateValue(name, key, count, db);
                }
            }

            if (value <= 0) throw new InvalidOperationException($"GetDbValue(name={name}, key={key})错误！");

            return value;
        }

        private List<string> GetValue(string name, string key, int count, int? cacheCount, int? formatNum)
        {
            if (!cacheCount.HasValue || cacheCount <= 0) cacheCount = this.CacheCount;
            List<string> vlist = new List<string>(cacheCount.Value);
            IdInfoModel vm = null;
            lock (this.dicLock)
            {
                dic.TryGetValue(name, out vm);
            }

            if (vm == null)
            {
                lock (this.dicLock)
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
                        while (vm.StratValue < vm.EndValue)
                        {
                            vm.StratValue = vm.StratValue + 1;
                            string v = FormatId(key, vm.StratValue, formatNum);
                            vlist.Add(v);
                        }
                        count = count - c;
                        vm.EndValue = GetDbValue(name, key, cacheCount.Value + count);
                        vm.StratValue = vm.EndValue - cacheCount.Value - count;
                    }
                }
                else
                {
                    vm.EndValue = GetDbValue(name, key, cacheCount.Value + count);
                    vm.StratValue = vm.EndValue - cacheCount.Value - count;
                    vm.Key = key;
                }

                while (count > 0)
                {
                    vm.StratValue = vm.StratValue + 1;
                    string v = FormatId(key, vm.StratValue, formatNum);
                    vlist.Add(v);
                    count--;
                }
            }

            return vlist;
        }

        private string GetName(Type type)
        {
            return type.Name;
        }

        private string GetKey(bool withDate, bool withServerId)
        {
            string key = withDate ? DateTime.Now.ToString("yyyyMMdd") : string.Empty;
            if (withServerId) key = $"{key}{this.ServerId}";

            return key;
        }

        private string FormatId(string key, int value, int? formatNum)
        {
            if (!formatNum.HasValue) formatNum = this.FormatNum;
            string format = "{0}{1:D" + (formatNum > 0 ? formatNum.ToString() : "") + "}";
            string id = string.Format(format, key, value);

            return id;
        }

        public string Get<T>(bool withDate = true, bool withServerId = true, int? formatNum = null, int? cacheCount = null) where T : class, IModel
        {
            return Get(typeof(T), withDate, withServerId, formatNum, cacheCount);
        }

        public string Get(Type type, bool withDate = true, bool withServerId = true, int? formatNum = null, int? cacheCount = null)
        {
            if (this.IsDisposed) throw new ObjectDisposedException(nameof(IdGenerator));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!type.IsClass) throw new ArgumentException($"{type.FullName} is not class!", nameof(type));
            if (formatNum.HasValue && formatNum.Value < 0) throw new ArgumentNullException(nameof(formatNum));
            if (!typeof(IModel).IsAssignableFrom(type)) throw new ArgumentException($"{type.FullName} is not IModel!", nameof(type));

            string name = GetName(type);
            string key = GetKey(withDate, withServerId);
            var list = GetValue(name, key, 1, cacheCount, formatNum);
            var id = list.FirstOrDefault();

            return id;
        }

        public List<string> GetList<T>(int count, bool withDate = true, bool withServerId = true, int? formatNum = null, int? cacheCount = null) where T : class, IModel
        {
            return this.GetList(typeof(T), count, withDate, withServerId, formatNum, cacheCount);
        }

        public List<string> GetList(Type type, int count, bool withDate = true, bool withServerId = true, int? formatNum = null, int? cacheCount = null)
        {
            if (this.IsDisposed) throw new ObjectDisposedException(nameof(IdGenerator));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (formatNum.HasValue && formatNum.Value < 0) throw new ArgumentNullException(nameof(formatNum));
            if (count <= 0) throw new ArgumentException($"{nameof(count)}({count}) is error!", nameof(count));
            if (!type.IsClass) throw new ArgumentException($"{type.FullName} is not class!", nameof(type));
            if (!typeof(IModel).IsAssignableFrom(type)) throw new ArgumentException($"{type.FullName} is not IModel!", nameof(type));

            string name = GetName(type);
            string key = GetKey(withDate, withServerId);
            var list = GetValue(name, key, count, cacheCount, formatNum);

            return list;
        }
    }
}
