using System;
using System.IO;
using System.Reflection;
using System.Xml;

using Afx.Cache;
using Afx.Utils;
using Afx.Configuration;
using AfxDotNetCoreSample.Common;
using StackExchange.Redis;

namespace AfxDotNetCoreSample.Web
{
    public static class IocRegister
    {
        const string IOC_REGISTER_FILE = "Config/IocRegister.xml";
        const string DEFAULT_IMPLEMENT_FILE = "Config/DefaultImplement.xml";
        const string CACHE_KEY_FILE = "Config/CacheKey.xml";

        public static void Register()
        {
            IocUtils.RegisterSingle<CacheKey>(new CacheKey(PathUtils.GetFileFullPath(CACHE_KEY_FILE)));

            IocUtils.LoadDefaultImplement(DEFAULT_IMPLEMENT_FILE);

            LoadIoc();

            LoadRedis();
        }

        private static void LoadIoc()
        {
            var filepath = PathUtils.GetFileFullPath(IOC_REGISTER_FILE);
            if (!File.Exists(filepath)) throw new FileNotFoundException(IOC_REGISTER_FILE + " is not found!", IOC_REGISTER_FILE);

            var container = IocUtils.DefaultContainer;
            container.AddGlobalAop<AopLog>();
            XmlDocument doc = new XmlDocument();
            using (var fs = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                doc.Load(fs);
            }

            if (doc.DocumentElement == null) throw new ArgumentException(IOC_REGISTER_FILE + " is error!");

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node is XmlElement && node.Name == "Register")
                {
                    var el = node as XmlElement;
                    var s = el.GetAttribute("base");
                    if (string.IsNullOrEmpty(s)) throw new ArgumentException(IOC_REGISTER_FILE + " is error!");
                    var v = el.GetAttribute("value");
                    if (string.IsNullOrEmpty(v)) throw new ArgumentException(IOC_REGISTER_FILE + " is error!");
                    var arr = s.Split(',');
                    if (arr.Length != 2) if (string.IsNullOrEmpty(s)) throw new ArgumentException(IOC_REGISTER_FILE + " is error!");
                    string name = arr[0].Trim();
                    string nametype = arr[1].Trim();
                    if (string.IsNullOrEmpty(name)) throw new ArgumentException(IOC_REGISTER_FILE + " is error!");
                    if (string.IsNullOrEmpty(nametype)) throw new ArgumentException(IOC_REGISTER_FILE + " is error!");
                    var baseAssembly = Assembly.Load(nametype);
                    var baseType = baseAssembly.GetType(name, true);
                    var classAssembly = Assembly.Load(v);
                    container.Register(baseType, classAssembly);
                }
            }
        }

        private static void LoadRedis()
        {
            var con = ConnectionMultiplexer.Connect(ConfigUtils.RedisConfig);
            con.ConnectionFailed += OnConnectionFailed;
            con.ErrorMessage += OnErrorMessage;
            con.InternalError += OnInternalError;
            con.PreserveAsyncOrder = false;

            IocUtils.RegisterSingle<IConnectionMultiplexer>(con);
        }

        private static void OnInternalError(object sender, InternalErrorEventArgs e)
        {
            LogUtils.Error($"【Redis.InternalError】ConnectionType:{e.ConnectionType}, EndPoint: {e.EndPoint}, Origin: {e.Origin}", e.Exception);
        }

        private static void OnErrorMessage(object sender, RedisErrorEventArgs e)
        {
            LogUtils.Error($"【Redis.InternalError】EndPoint: {e.EndPoint}, error: {e.Message}");
        }

        private static void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            LogUtils.Error($"【Redis.InternalError】ConnectionType:{e.ConnectionType}, EndPoint: {e.EndPoint}, FailureType: {e.FailureType}", e.Exception);
        }
    }
}