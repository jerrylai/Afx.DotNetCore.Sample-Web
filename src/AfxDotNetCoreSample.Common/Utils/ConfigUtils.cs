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
                if (string.IsNullOrEmpty(s)) throw new ArgumentNullException("HostUrl");
                return s;
            }
        }

        public static long MultipartBodyLengthLimit
        {
            get
            {
                long length = 0;
                var s = GetValue("MultipartBodyLengthLimit");
                if (!long.TryParse(s, out length) || length < 1 * 1024 * 1024)
                {
                    length = 1 * 1024 * 1024;
                }

                return length;
            }
        }

        public static string RedisConfig
        {
            get
            {
                var s = GetValue("RedisConfig");
                if (string.IsNullOrEmpty(s)) throw new ArgumentNullException("RedisConfig");

                return s;
            }
        }

        public static string DesKey
        {
            get
            {
                var s = GetValue("Encrypt:DesKey");
                if (string.IsNullOrEmpty(s)) throw new ArgumentNullException("DesKey");
                if (s.Length != 32) throw new ArgumentException("DesKey.Length is error!", "DesKey");
                return s.Substring(0, 24);
            }
        }

        #region Database
        public static bool InitDatabase
        {
            get
            {
                bool temp = false;
                string s = GetValue("Database:Init");
                if (string.IsNullOrEmpty(s) || !bool.TryParse(s, out temp))
                {
                    temp = false;
                }

                return temp;
            }
        }

        public static bool IsWriteSqlLog
        {
            get
            {
                bool temp = false;
                string s = GetValue("Database:IsLog");
                if (string.IsNullOrEmpty(s) || !bool.TryParse(s, out temp))
                {
                    temp = false;
                }

                return temp;
            }
        }

        public static DatabaseType DatabaseType
        {
            get
            {
                DatabaseType databaseType = DatabaseType.None;
                string s = GetValue("Database:Type");
                if (string.IsNullOrEmpty(s) || !Enum.TryParse(s, true, out databaseType) || databaseType == DatabaseType.None)
                {
                    throw new ArgumentNullException("DatabaseType");
                }

                return databaseType;
            }
        }

        public static string ConnectionString
        {
            get
            {
                string key = $"Database:{DatabaseType}";
                var s = GetValue(key);
                if (string.IsNullOrEmpty(s))
                {
                    throw new ArgumentNullException(key);
                }

                return s;
            }
        }
        #endregion

        #region Cache

        public static CacheType CacheType
        {
            get
            {
                var val = CacheType.None;
                string s = GetValue("Cache:Type");
                if (string.IsNullOrEmpty(s) || !Enum.TryParse(s, true, out val))
                {
                    throw new ArgumentNullException("CacheType");
                }

                return val;
            }
        }

        public static string CachePrefix
        {
            get
            {
                    var s = GetValue("Cache:Prefix") ?? "";

                return s;
            }
        }
        #endregion

        public static int IdGeneratorCount
        {
            get
            {
                int val = 0;
                var s = GetValue("IdGenerator:CacheCount");
                if (int.TryParse(s, out val) || val <= 0)
                {
                    val = 20;
                }
                return val;
            }
        }

        public static string IdGeneratorServerId
        {
            get
            {
                var s = GetValue("IdGenerator:ServerId");
                if (string.IsNullOrEmpty(s)) s = "1001";

                return s;
            }
        }

        public static int IdGeneratorFormatNum
        {
            get
            {
                int val = 0;
                var s = GetValue("IdGenerator:FormatNum");
                if (int.TryParse(s, out val) || val <= 0)
                {
                    val = 8;
                }
                return val;
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
