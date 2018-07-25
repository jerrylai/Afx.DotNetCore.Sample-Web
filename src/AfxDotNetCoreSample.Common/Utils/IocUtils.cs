using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Afx.Ioc;
using Afx.Utils;

namespace AfxDotNetCoreSample.Common
{
    public static class IocUtils
    {
        private static Lazy<IocContainer> _defaultContainer = new Lazy<IocContainer>(() =>
        {
            var ioc = new IocContainer(true);
            ioc.AddGlobalAop<AopLog>();

            return ioc;
        }, false);

        public static IocContainer Default
        {
            get { return _defaultContainer.Value; }
        }

        private static Lazy<Dictionary<string, string>> _iocConfigDic = new Lazy<Dictionary<string, string>>(() =>
        {
            var dic = new Dictionary<string, string>();
            string filepath = PathUtils.GetFileFullPath("Config/IocConfig.xml");
            if (File.Exists(filepath))
            {
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
                                    dic[key] = value;
                                }
                            }
                        }
                    }
                }
            }

            return dic;
        }, true);
        private static Dictionary<string, string> IocConfigDic => _iocConfigDic.Value;

        public static T Get<T>() => Get<T>(null, null);

        public static T Get<T>(string name) => Get<T>(name, null);

        public static T Get<T>(object[] args) => Get<T>(null, args);

        public static T Get<T>(string name, object[] args)
        {
            T result = default(T);

            if(string.IsNullOrEmpty(name))
            {
                var key = typeof(T).FullName;
                IocConfigDic.TryGetValue(key, out name);
            }

            result = Default.Get<T>(name, args);

            if(result == null)
            {
                throw new ArgumentException($"未找到 { typeof(T).FullName } 实现类（name={name}）!", typeof(T).FullName);
            }

            return result;
        }

        
    }
}
