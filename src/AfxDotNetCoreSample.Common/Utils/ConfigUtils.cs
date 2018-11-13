using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Common
{
    public static class ConfigUtils
    {
        private static Lazy<IConfiguration> _configuration = new Lazy<IConfiguration>(() =>
         IocUtils.Get<IConfiguration>(), false);

        public static IConfiguration Configuration => _configuration.Value;

        public static string GetValue(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            string value = Configuration[key];

            return value;
        }

        public static string WebFileDirectory
        {
            get
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "webfiles");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string TempDirectory
        {
            get
            {
                string path = Path.Combine(WebFileDirectory, "temp");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }

        public static void SetThreads()
        {
            int ProcessorCount = Environment.ProcessorCount;
            LogUtils.Debug($"【SetThreads】ProcessorCount: {ProcessorCount}");
            int minThreads = ProcessorCount * 2;
            int minIoThreads = ProcessorCount * 2;

            if (minThreads < 10) minThreads = 5;
            if (minIoThreads < 10) minIoThreads = 5;

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

            LogUtils.Debug($"【SetThreads->Config】minThreads: {minThreads}, minIoThreads: {minIoThreads}");

            //GetMaxThreads
            int maxThreads = 0;
            int maxIoThreads = 0;
            System.Threading.ThreadPool.GetMaxThreads(out maxThreads, out maxIoThreads);
            LogUtils.Debug($"【SetThreads->GetMaxThreads】maxThreads: {maxThreads}, maxIoThreads: {maxIoThreads}");
            //SetMinThreads
            int workerThreads = 0;
            int completionPortThreads = 0;
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

        public static string JsVersion
        {
            get
            {
                var s = GetValue("JsVersion");
                if (string.IsNullOrEmpty(s)) s = "v1.0.0.0";
                return s;
            }
        }

        public static string HostUrl
        {
            get
            {
                var s = GetValue("HostUrl");
                if (string.IsNullOrEmpty(s)) s = "http://wx.huayacnc.com/";
                return s;
            }
        }

        private static long _multipartBodyLengthLimit = 0;
        public static long MultipartBodyLengthLimit
        {
            get
            {
                if (_multipartBodyLengthLimit == 0)
                {
                    long length = 0;
                    var s = GetValue("MultipartBodyLengthLimit");
                    if (!long.TryParse(s, out length) || length < 1 * 1024 * 1024)
                    {
                        length = 1 * 1024 * 1024;
                    }
                    _multipartBodyLengthLimit = length;
                }

                return _multipartBodyLengthLimit;
            }
        }

        private static List<string> _mageFileExtension;
        public static List<string> ImageFileExtension
        {
            get
            {
                if (_mageFileExtension == null)
                {
                    var s = GetValue("ImageFileExtension");
                    if (string.IsNullOrEmpty(s)) s = ".png,.jpg,.jpeg,.bmp,.gif,.icon,.tiff,.exif,.emf,.wmf";
                    var arr = s.Split(',');
                    var list = new List<string>(arr.Length);
                    foreach(var item in arr)
                    {
                        if (!string.IsNullOrEmpty(item.Trim())) list.Add(item.Trim());
                    }
                    _mageFileExtension = list;
                }

                return _mageFileExtension;
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

        private static int _idGeneratorFormatNum = 0;
        public static int IdGeneratorFormatNum
        {
            get
            {
                if (_idGeneratorFormatNum == 0)
                {
                    var s = GetValue("IdGenerator:FormatNum");
                    if (int.TryParse(s, out _idGeneratorFormatNum) || _idGeneratorFormatNum <= 0)
                    {
                        _idGeneratorFormatNum = 8;
                    }
                }
                return _idGeneratorFormatNum;
            }
        }


        public static int LogSaveDay
        {
            get
            {
                int temp = 0;
                var s = GetValue("LogSaveDay");
                if (!int.TryParse(s, out temp) || temp < 1)
                {
                    temp = 1;
                }

                return temp;
            }
        }
    }
}
