using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

using Afx.Cache;
using Afx.Ioc;
using Afx.Utils;
using AfxDotNetCoreSample.Common;
using Microsoft.Extensions.Configuration;

namespace AfxDotNetCoreSample.Web
{
    public static class IocConfig
    {
        const string IOC_CONFIG_FILE = "Config/IocConfig.xml";
        const string AOP_CONFIG_FILE = "Config/AopConfig.xml";
        const string DEFAULT_IMPLEMENT_FILE = "Config/DefaultImplement.xml";
        const string CACHE_KEY_FILE = "Config/CacheKey.xml";

        public static void Register(IConfiguration configuration)
        {
            IocUtils.LoadDefaultImplement(DEFAULT_IMPLEMENT_FILE);
            IocUtils.Default.Register(configuration);

            LoadAll();
        }

        private static void LoadAll()
        {
            var container = IocUtils.Default;

            container.Register(new CacheKey(PathUtils.GetFileFullPath(CACHE_KEY_FILE)));
            container.Register<AopLog>();
            container.Register(c => RedisUtils.Default);

            LoadAssembly(container);

            container.Register<Dto.ISyncLock, Service.SyncLock>();
        }

        private static void LoadAssembly(IContainer container)
        {
            var filepath = PathUtils.GetFileFullPath(IOC_CONFIG_FILE);
            if (!File.Exists(filepath)) throw new FileNotFoundException(IOC_CONFIG_FILE + " is not found!", IOC_CONFIG_FILE);

            XmlDocument doc = new XmlDocument();
            using (var fs = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                doc.Load(fs);
            }

            if (doc.DocumentElement == null) throw new ArgumentException(IOC_CONFIG_FILE + " is error!");

            var aopConfigDic = GetAopConfig();
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node is XmlElement && node.Name == "Register")
                {
                    var el = node as XmlElement;
                    var baseInterfaceAttr = el.GetAttribute("baseInterface");
                    if (string.IsNullOrEmpty(baseInterfaceAttr)) throw new ArgumentException(IOC_CONFIG_FILE + " is error!");
                    var assemblyAttr = el.GetAttribute("assembly");
                    if (string.IsNullOrEmpty(assemblyAttr)) throw new ArgumentException(IOC_CONFIG_FILE + " is error!");
                    var arr = baseInterfaceAttr.Split(',');
                    if (arr.Length != 2) throw new ArgumentException(IOC_CONFIG_FILE + " is error!");
                    string baseInterfaceName = arr[0].Trim();
                    string baseInterfaceAssemblyName = arr[1].Trim();
                    if (string.IsNullOrEmpty(baseInterfaceName)) throw new ArgumentException(IOC_CONFIG_FILE + " is error!");
                    if (string.IsNullOrEmpty(baseInterfaceAssemblyName)) throw new ArgumentException(IOC_CONFIG_FILE + " is error!");
                    var baseInterfaceAssembly = Assembly.Load(baseInterfaceAssemblyName);
                    var baseInterfaceType = baseInterfaceAssembly.GetType(baseInterfaceName, true);
                    var classAssembly = Assembly.Load(assemblyAttr);
                    var list = container.Register(baseInterfaceType, classAssembly);

                    var aopEnabledAttr = el.GetAttribute("aopEnabled");
                    bool aopEnabled = "true".Equals(aopEnabledAttr, StringComparison.OrdinalIgnoreCase) || aopEnabledAttr == "1";
                    var aopTypeAttr = el.GetAttribute("aopType");
                    arr = (aopTypeAttr ?? "").Split(',');
                    Type aopType = null;
                    if (arr.Length == 2)
                    {
                        var aopTypeName = arr[0].Trim();
                        var aopTypeAssemblyName = arr[1].Trim();
                        if (string.IsNullOrEmpty(aopTypeName)) throw new ArgumentException(IOC_CONFIG_FILE + " is error!");
                        if (string.IsNullOrEmpty(aopTypeAssemblyName)) throw new ArgumentException(IOC_CONFIG_FILE + " is error!");
                        var aopTypeAssembly = Assembly.Load(aopTypeAssemblyName);
                        aopType = aopTypeAssembly.GetType(aopTypeName, true);
                    }
                    foreach (var registerContext in list)
                    {
                        string name = registerContext.Context.TargetInfo.TargetType.FullName;
                        registerContext.Name(name);
                        AopConfig aopConfig = null;
                        if (aopConfigDic.TryGetValue(name, out aopConfig))
                        {
                            registerContext.Aop(aopConfig.Enabled);
                            if (aopConfig.Type != null) registerContext.Aop(aopConfig.Type);
                        }
                        else
                        {
                            registerContext.Aop(aopEnabled);
                            if (aopType != null) registerContext.Aop(aopType);
                        }
                    }
                }
            }
        }

        private static Dictionary<string, AopConfig> GetAopConfig()
        {
            Dictionary<string, AopConfig> dic = new Dictionary<string, AopConfig>();
            var filepath = PathUtils.GetFileFullPath(AOP_CONFIG_FILE);
            if (!File.Exists(filepath)) throw new FileNotFoundException(AOP_CONFIG_FILE + " is not found!", AOP_CONFIG_FILE);

            XmlDocument doc = new XmlDocument();
            using (var fs = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                doc.Load(fs);
            }

            if (doc.DocumentElement == null) throw new ArgumentException(AOP_CONFIG_FILE + " is error!");

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node is XmlElement && node.Name == "Aop")
                {
                    var el = node as XmlElement;
                    var nameAttr = el.GetAttribute("name");
                    if (string.IsNullOrEmpty(nameAttr)) throw new ArgumentException(AOP_CONFIG_FILE + " is error!");
                    var enabledAttr = el.GetAttribute("enabled");
                    var typeAttr = el.GetAttribute("type");
                    var arr = (typeAttr ?? "").Split(',');
                    Type aopType = null;
                    if (arr.Length == 2)
                    {
                        string aopTypeName = arr[0].Trim();
                        string aopTypeAssemblyName = arr[1].Trim();
                        if (string.IsNullOrEmpty(aopTypeName)) throw new ArgumentException(AOP_CONFIG_FILE + " is error!");
                        if (string.IsNullOrEmpty(aopTypeAssemblyName)) throw new ArgumentException(AOP_CONFIG_FILE + " is error!");
                        var aopTypeAssembly = Assembly.Load(aopTypeAssemblyName);
                        aopType = aopTypeAssembly.GetType(aopTypeName, true);
                    }

                    dic[nameAttr] = new AopConfig()
                    {
                        Name = nameAttr,
                        Enabled = "true".Equals(enabledAttr, StringComparison.OrdinalIgnoreCase) || enabledAttr == "1",
                        Type = aopType
                    };
                }
            }

            return dic;
        }

        class AopConfig
        {
            public string Name { get; set; }

            public bool Enabled { get; set; }

            public Type Type { get; set; }
        }
    }
}