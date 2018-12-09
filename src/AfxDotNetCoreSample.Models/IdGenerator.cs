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

        /// <summary>
        /// 获取id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string Get<T>() where T : class, IModel;

        /// <summary>
        /// 获取id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string Get(Type type);

        /// <summary>
        /// 获取id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<string> GetList<T>(int count) where T : class, IModel;

        /// <summary>
        /// 批量获取id
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count">获取个数</param>
        /// <returns></returns>
        List<string> GetList(Type type, int count);
    }

    /// <summary>
    /// 分布式id生成器 add by jerrylai@liyun.com
    /// </summary>
    /// <summary>
    /// 分布式id生成器 add by jerrylai@liyun.com
    /// </summary>
    public sealed class IdGenerator : IIdGenerator, IDisposable
    {
        class IdInfoModel
        {
            public string Id { get; set; }

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

        private ReadWriteLock rwLock;
        private Dictionary<string, IdInfoModel> dic;
        private IdInfoModel sequenceModel;
        private readonly string selectIdSQL;
        private readonly string updateSQL;
        private readonly string upSelectSQL;
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
                if (this.rwLock != null) this.rwLock.Dispose();
                this.rwLock = null;
                this.sequenceModel = null;
            }
        }

        public IdGenerator(DatabaseType type, string connectionString, string serverId = "1001", int cacheCount = 1000, int formatNum = 8)
        {
            this.IsDisposed = true;
            if (type == DatabaseType.None) throw new ArgumentException($"{nameof(type)} is error!", nameof(type));
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            if (string.IsNullOrEmpty(serverId)) throw new ArgumentNullException(nameof(connectionString));
            if (0 >= cacheCount) throw new ArgumentException($"{nameof(cacheCount)} is error!", nameof(cacheCount));
            if (0 > formatNum) throw new ArgumentException($"{nameof(formatNum)} is error!", nameof(formatNum));
            this.IsDisposed = false;
            this.rwLock = new ReadWriteLock();
            this.dic = new Dictionary<string, IdInfoModel>();
            this.DatabaseType = type;
            this.ConnectionString = connectionString;
            this.ServerId = serverId;
            this.CacheCount = cacheCount;
            this.FormatNum = formatNum;
            this.sequenceModel = new IdInfoModel() { Id = serverId, Key = serverId, StratValue = 0, EndValue = 0 };

            switch (type)
            {
                case DatabaseType.MSSQLServer:
                    selectIdSQL = "SELECT [Id] FROM [SysSequence] WHERE [Name] = @Name AND [Key] = @Key;";

                    updateSQL = "UPDATE [SysSequence] SET [Value] = [Value] + @Count, [UpdateTime] = @Now WHERE [Id] = @Id;";
                    upSelectSQL = "SELECT [Value] FROM [SysSequence] WHERE [Id] = @Id;";

                    insertSQL = "INSERT INTO [SysSequence]([Id], [Name], [Key], [Value], [UpdateTime], [CreateTime]) VALUES(@Id, @Name, @Key, @Value, @UpdateTime, @CreateTime);";
                    break;
                case DatabaseType.MySQL:
                    selectIdSQL = "SELECT `Id` FROM `SysSequence` WHERE `Name` = @Name AND `Key` = @Key;";

                    updateSQL = "UPDATE `SysSequence` SET `Value` = `Value` + @Count, `UpdateTime` = @Now WHERE `Id` = @Id;";
                    upSelectSQL = "SELECT `Value` FROM `SysSequence` WHERE `Id` = @Id;";

                    insertSQL = "INSERT INTO `SysSequence`(`Id`, `Name`, `Key`, `Value`, `UpdateTime`, `CreateTime`) VALUES(@Id, @Name, @Key, @Value, @UpdateTime, @CreateTime);";
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
                    return new MySQLDatabase(this.ConnectionString);
                default:
                    throw new Exception($"不支持 {this.DatabaseType} 数据库！");
            }
        }

        private string GetIdentity(IDatabase db)
        {
            int value = 0;
            lock (this.sequenceModel.LockObj)
            {
                if (this.sequenceModel.StratValue >= this.sequenceModel.EndValue)
                {
                    int count = 10;
                    this.sequenceModel.EndValue = this.UpdateValue(db, this.ServerId, count);
                    if (this.sequenceModel.EndValue <= 0)
                    {
                        var now = DateTime.Now;
                        var m = new SysSequence { Id = this.ServerId, Name = "0", Key = "0", Value = count, UpdateTime = now, CreateTime = now };
                        try
                        {
                            var c = db.ExecuteNonQuery(this.insertSQL, m);
                            this.sequenceModel.EndValue = count;
                        }
                        catch (Exception ex)
                        {
                            this.sequenceModel.EndValue = this.UpdateValue(db, this.ServerId, count);
                        }
                    }
                    this.sequenceModel.StratValue = this.sequenceModel.EndValue - count;
                }
                this.sequenceModel.StratValue = this.sequenceModel.StratValue + 1;
                value = this.sequenceModel.StratValue;
            }

            return string.Format("{0}{1:D8}", this.ServerId, value);
        }

        private string GetId(IDatabase db, string name, string key)
        {
            string id = null;
            using (db.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                id = db.ExecuteScalar<string>(this.selectIdSQL, new { Name = name, Key = key });
                db.Commit();
            }

            return id;
        }

        private int UpdateValue(IDatabase db, string id, int count)
        {
            int value = -1;
            using (db.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                int c = db.ExecuteNonQuery(this.updateSQL, new { Count = count, Now = DateTime.Now, Id = id });
                if (c > 0)
                {
                    value = db.ExecuteScalar<int>(upSelectSQL, new { Id = id });
                }
                db.Commit();
            }

            return value;
        }

        private string InsertVaule(IDatabase db, string name, string key, int count)
        {
            string id = null;
            var now = DateTime.Now;
            var m = new SysSequence { Name = name, Key = key, Value = count, UpdateTime = now, CreateTime = now };
            m.Id = this.GetIdentity(db);
            try
            {
                var c = db.ExecuteNonQuery(this.insertSQL, m);
                id = m.Id;
            }
            catch (Exception ex)
            {

            }

            return id;
        }

        private int GetDbValue(string name, string key, int count, ref string id)
        {
            int value = -1;

            using (var db = GetDb())
            {
                if (string.IsNullOrEmpty(id)) id = GetId(db, name, key);
                if (string.IsNullOrEmpty(id))
                {
                    id = InsertVaule(db, name, key, count);
                    if (!string.IsNullOrEmpty(id))
                    {
                        value = count;
                    }
                }

                if (value <= 0)
                {
                    if (string.IsNullOrEmpty(id)) id = GetId(db, name, key);
                    value = UpdateValue(db, id, count);
                }
            }

            return value;
        }

        private int GetValue(string name, string key, int count)
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
                string id = vm.Id;
                if (string.Equals(vm.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    int c = vm.EndValue - vm.StratValue;
                    if (c < count)
                    {
                        vm.EndValue = GetDbValue(name, key, CacheCount + count - c, ref id);
                    }
                }
                else
                {
                    id = null;
                    vm.EndValue = GetDbValue(name, key, CacheCount + count, ref id);
                    vm.StratValue = vm.EndValue - CacheCount - count;
                    vm.Key = key;
                }
                vm.Id = id;
                vm.StratValue = vm.StratValue + count;
                value = vm.StratValue;
            }

            return value;
        }

        private string GetName(Type type)
        {
            return type.Name;
        }

        private string GetKey(string name)
        {
            string key = $"{DateTime.Now.ToString("yyyyMMdd")}{ServerId}";

            return key;
        }

        private string FormatId(string key, int value)
        {
            string format = "{0}{1:D" + (this.FormatNum > 1 ? this.FormatNum.ToString() : "") + "}";
            string id = string.Format(format, key, value);

            return id;
        }

        /// <summary>
        /// 获取id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string Get<T>() where T : class, IModel
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// 获取id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string Get(Type type)
        {
            if (this.IsDisposed) throw new ObjectDisposedException(nameof(IdGenerator));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!type.IsClass) throw new ArgumentException($"{type.FullName} is not class!", nameof(type));
            if (!typeof(IModel).IsAssignableFrom(type)) throw new ArgumentException($"{type.FullName} is not IModel!", nameof(type));

            string id = null;
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
        public List<string> GetList<T>(int count) where T : class, IModel
        {
            return this.GetList(typeof(T), count);
        }

        /// <summary>
        /// 批量获取id
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count">获取个数</param>
        /// <returns></returns>
        public List<string> GetList(Type type, int count)
        {
            if (this.IsDisposed) throw new ObjectDisposedException(nameof(IdGenerator));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (count <= 0) throw new ArgumentException($"{nameof(count)}({count}) is error!", nameof(count));
            if (!type.IsClass) throw new ArgumentException($"{type.FullName} is not class!", nameof(type));
            if (!typeof(IModel).IsAssignableFrom(type)) throw new ArgumentException($"{type.FullName} is not IModel!", nameof(type));

            List<string> list = new List<string>(count);
            string name = GetName(type);
            string key = GetKey(name);
            int value = GetValue(name, key, count);
            list = new List<string>(count);
            for (int i = value - count + 1; i <= value; i++)
            {
                string id = FormatId(key, i);
                list.Add(id);
            }

            return list;
        }
    }
}
