using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Afx.Utils;
using Afx.Configuration;
using Microsoft.Extensions.Configuration;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Common
{
    public static class ConfigUtils
    {
        const string CONFIG_PREFIX = "AfxDotNetCoreSample:";

        private static Lazy<IConfiguration> _configuration = new Lazy<IConfiguration>(() =>
         {
             var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
             if (string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                "Development", StringComparison.OrdinalIgnoreCase))
             {
                 configBuilder = configBuilder.AddJsonFile("appsettings.Development.json");
             }
             configBuilder = configBuilder.AddEnvironmentVariables();
             var config = configBuilder.SetBasePath(Directory.GetCurrentDirectory()).Build();
             return config;
         }, true);

        public static IConfiguration Configuration => _configuration.Value;

        public static string GetValue(string key, bool isPrefix = true)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            string s = isPrefix ? CONFIG_PREFIX + key : key;
            string value = Configuration[s];

            return value;
        }

        public static string TempDirectory
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "temp");
            }
        }

        public static void SetThreads()
        {
            int ProcessorCount = Environment.ProcessorCount;
            LogUtils.Debug($"【SetThreads】ProcessorCount: {ProcessorCount}");
            int minThreads = ProcessorCount * 2;
            int minIoThreads = ProcessorCount * 2;
            int maxThreads = ProcessorCount * 1000;
            int maxIoThreads = 1000;

            if (minThreads < 10) minThreads = 10;
            if (minIoThreads < 10) minIoThreads = 10;
            if (maxThreads < 1000) maxThreads = 1000;

            var s = GetValue("Threads:Min");
            if (!string.IsNullOrEmpty(s))
            {
                int temp = 0;
                if (int.TryParse(s, out temp) && temp > minThreads)
                {
                    minThreads = temp;
                }
            }

            s = GetValue("Threads:IO");
            if (!string.IsNullOrEmpty(s))
            {
                int temp = 0;
                if (int.TryParse(s, out temp) && temp > minIoThreads)
                {
                    minIoThreads = temp;
                }
            }

            LogUtils.Debug($"【SetThreads】minThreads: {minThreads}, minIoThreads: {minIoThreads}, maxThreads: {maxThreads}, minIoThreads: {minIoThreads}");

            int workerThreads = 0;
            int completionPortThreads = 0;
            //SetMaxThreads
            System.Threading.ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            LogUtils.Debug($"【SetThreads->GetMaxThreads】workerThreads: {workerThreads}, completionPortThreads: {completionPortThreads}");
            if (workerThreads < maxThreads) workerThreads = maxThreads;
            if (completionPortThreads < maxIoThreads) completionPortThreads = maxIoThreads;
            System.Threading.ThreadPool.SetMaxThreads(workerThreads, completionPortThreads);
            LogUtils.Debug($"【SetThreads->SetMaxThreads】workerThreads: {workerThreads}, completionPortThreads: {completionPortThreads}");
            //SetMinThreads
            System.Threading.ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            LogUtils.Debug($"【SetThreads->GetMinThreads】workerThreads: {workerThreads}, completionPortThreads: {completionPortThreads}");
            if (workerThreads < minThreads) workerThreads = minThreads;
            if (completionPortThreads < minIoThreads) completionPortThreads = minIoThreads;
            System.Threading.ThreadPool.SetMinThreads(workerThreads, completionPortThreads);
            LogUtils.Debug($"【SetThreads->SetMinThreads】workerThreads: {workerThreads}, completionPortThreads: {completionPortThreads}");
        }

        public static string[] ServerUrls
        {
            get
            {
                var s = GetValue("ServerUrls");
                if (string.IsNullOrEmpty(s)) throw new ArgumentNullException("ServerUrls");
                var arr = s.Split(';');
                List<string> list = new List<string>(arr.Length);
                foreach (var v in arr) if (!string.IsNullOrEmpty(v.Trim())) list.Add(v.Trim());
                return list.ToArray();
            }
        }

        private static string _redisConfig;
        public static string RedisConfig
        {
            get
            {
                if (string.IsNullOrEmpty(_redisConfig))
                {
                    _redisConfig = GetValue("RedisConfig");
                    if (string.IsNullOrEmpty(_redisConfig)) throw new ArgumentNullException("RedisConfig");
                }

                return _redisConfig;
            }
        }

        private static string _desKey;
        public static string DesKey
        {
            get
            {
                if (string.IsNullOrEmpty(_desKey))
                {
                    _desKey = GetValue("Encrypt:DesKey");
                    if (string.IsNullOrEmpty(_desKey)) throw new ArgumentNullException("DesKey");
                }

                return _desKey;
            }
        }

        #region Database
        private static Nullable<bool> _initDatabase = null;
        public static bool InitDatabase
        {
            get
            {
                if (!_initDatabase.HasValue)
                {
                    bool temp = true;
                    string s = GetValue("Database:Init");
                    if (!string.IsNullOrEmpty(s) && bool.TryParse(s, out temp))
                    {
                        _initDatabase = temp;
                    }
                    else
                    {
                        _initDatabase = false;
                    }
                }

                return _initDatabase.Value;
            }
        }

        private static Nullable<bool> _isWriteSqlLog = null;
        public static bool IsWriteSqlLog
        {
            get
            {
                if (!_isWriteSqlLog.HasValue)
                {
                    bool temp = false;
                    string s = GetValue("Database:IsLog");
                    if (!string.IsNullOrEmpty(s) && bool.TryParse(s, out temp))
                    {
                        _isWriteSqlLog = temp;
                    }
                    else
                    {
                        _isWriteSqlLog = false;
                    }
                }

                return _isWriteSqlLog.Value;
            }
        }

        private static DatabaseType _databaseType = DatabaseType.None;
        public static DatabaseType DatabaseType
        {
            get
            {
                if (_databaseType == DatabaseType.None)
                {
                    string s = GetValue("Database:Type");
                    if (string.IsNullOrEmpty(s) || !Enum.TryParse(s, true, out _databaseType) || _databaseType == DatabaseType.None)
                    {
                        throw new ArgumentNullException("DatabaseType");
                    }
                }

                return _databaseType;
            }
        }

        private static string _connectionString = null;
        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    string key = $"Database:{DatabaseType}";
                    _connectionString = GetValue(key);
                    if (string.IsNullOrEmpty(_connectionString))
                    {
                        throw new ArgumentNullException(key);
                    }
                }

                return _connectionString;
            }
        }
        #endregion

        #region Cache

        private static Nullable<CacheType> _cacheType = null;
        public static CacheType CacheType
        {
            get
            {
                if (!_cacheType.HasValue)
                {
                    var val = CacheType.None;
                    string s = GetValue("Cache:Type");
                    if (string.IsNullOrEmpty(s) ||  !Enum.TryParse(s, true, out val))
                    {
                        throw new ArgumentNullException("CacheType");
                    }
                    else
                    {
                        _cacheType = val;
                    }
                }

                return _cacheType.Value;
            }
        }

        private static string _cachePrefix;
        public static string CachePrefix
        {
            get
            {
                if (_cachePrefix == null)
                {
                    _cachePrefix = GetValue("Cache:Prefix") ?? "";
                }

                return _cachePrefix;
            }
        }
        #endregion

        private static int _idGeneratorCount = 0;
        public static int IdGeneratorCount
        {
            get
            {
                if(_idGeneratorCount == 0)
                {
                    var s = GetValue("IdGenerator:CacheCount");
                    if(int.TryParse(s, out _idGeneratorCount) || _idGeneratorCount <= 0)
                    {
                        _idGeneratorCount = 10;
                    }
                }
                return _idGeneratorCount;
            }
        }

        private static string _idGeneratorServerId;
        public static string IdGeneratorServerId
        {
            get
            {
                if (_idGeneratorServerId == null)
                {
                    _idGeneratorServerId = GetValue("IdGenerator:ServerId") ?? "";
                }
                return _idGeneratorServerId;
            }
        }

        public static int LogSaveDay = 7;
    }
}
