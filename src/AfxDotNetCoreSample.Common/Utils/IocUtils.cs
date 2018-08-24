using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Afx.Cache;
using Afx.Ioc;
using Afx.Utils;

namespace AfxDotNetCoreSample.Common
{
    public static class IocUtils
    {
        private static IContainer _container = new Container();

        public static IContainer Default => _container;

        private static Dictionary<string, string> _iocConfigDic = new Dictionary<string, string>();

        private static Dictionary<string, string> IocConfigDic => _iocConfigDic;

        public static void LoadDefaultImplement(string defaultImplementFile)
        {
            if (string.IsNullOrEmpty(defaultImplementFile)) throw new ArgumentNullException(nameof(defaultImplementFile));
            string filepath = PathUtils.GetFileFullPath(defaultImplementFile);
            if (!File.Exists(filepath)) throw new FileNotFoundException("file(" + defaultImplementFile + ") not found!");

            XmlDocument doc = new XmlDocument();
            using (var fs = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                doc.Load(fs);
            }

            if (doc.DocumentElement != null)
            {
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (node is XmlElement)
                    {
                        var element = node as XmlElement;
                        if (element.Name == "Interface")
                        {
                            var key = element.GetAttribute("name");
                            var value = element.GetAttribute("value");
                            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                            {
                                IocConfigDic[key] = value;
                            }
                        }
                    }
                }
            }
        }

        public static void ClearConfig()
        {
            IocConfigDic.Clear();
        }

        public static TService Get<TService>() => Get<TService>(null, null);

        public static TService Get<TService>(string name) => Get<TService>(name, null);

        public static TService Get<TService>(object[] args) => Get<TService>(null, args);

        public static TService Get<TService>(string name, object[] args)
        {
            TService result = default(TService);

            if (string.IsNullOrEmpty(name))
            {
                var key = typeof(TService).FullName;
                IocConfigDic.TryGetValue(key, out name);
            }

            if (!string.IsNullOrEmpty(name))
            {
                result = Default.GetByName<TService>(name, args);
            }
            else
            {
                result = Default.Get<TService>(args);
            }

            if (result == null)
            {
                throw new ArgumentException($"未找到 { typeof(TService).FullName } 实现类（name={name}）!", typeof(TService).FullName);
            }

            return result;
        }
    }
}
