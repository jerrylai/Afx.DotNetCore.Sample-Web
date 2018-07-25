using AfxDotNetCoreSample.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AfxDotNetCoreSample.Web
{
    public static class IocConfig
    {
        public static void Load()
        {
            var config = ConfigUtils.GetValue("Ioc:Service");
            if (string.IsNullOrEmpty(config)) throw new ArgumentNullException("Ioc->Service");
            var arr = config.Split(';');
            foreach (var s in arr)
            {
                if (!string.IsNullOrEmpty(s)) IocUtils.Default.Register<AfxDotNetCoreSample.IService.IBaseService>(s);
            }

            config = ConfigUtils.GetValue("Ioc:Repository");
            if (string.IsNullOrEmpty(config)) throw new ArgumentNullException("Ioc->Repository");
            arr = config.Split(';');
            foreach (var s in arr)
            {
                if (!string.IsNullOrEmpty(s)) IocUtils.Default.Register<AfxDotNetCoreSample.IRepository.IBaseRepository>(s);
            }

            config = ConfigUtils.GetValue("Ioc:Cache");
            if (string.IsNullOrEmpty(config)) throw new ArgumentNullException("Ioc->Cache");
            arr = config.Split(';');
            foreach (var s in arr)
            {
                if (!string.IsNullOrEmpty(s)) IocUtils.Default.Register<AfxDotNetCoreSample.ICache.IBaseCache>(s);
            }
        }
    }
}
