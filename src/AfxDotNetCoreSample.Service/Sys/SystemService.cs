using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Common;

namespace AfxDotNetCoreSample.Service
{
    public class SystemService : BaseService, ISystemService
    {
        protected virtual ISystemRepository systemRepository => this.GetRepository<ISystemRepository>();

        protected virtual IConfigRepository configRepository => this.GetRepository<IConfigRepository>();

        private Version version;

        public SystemService()
        {
            var assl = Assembly.GetAssembly(typeof(SystemService));
            var assleName = assl.GetName();
            this.version = assleName.Version;
        }

        /// <summary>
        /// 初始化系统
        /// </summary>
        /// <returns></returns>
        public virtual void Init()
        {
            try { systemRepository.InitDb(); }
            catch(Exception ex)
            {
                LogUtils.Info("【InitDatabase】", ex);
            }
            if (this.IsInitSystemData())
            {
                using (var syncLock = IocUtils.Get<ISyncLock>())
                {
                    syncLock.Init(LockType.InitSystemData, "0", null, TimeSpan.FromHours(1));
                    if (syncLock.Lock())
                    {
                        systemRepository.InitData();
                        this.SetInitSystemData();
                    }
                }
            }
        }

        /// <summary>
        /// 是否初始化系统数据
        /// </summary>
        /// <returns></returns>
        private bool IsInitSystemData()
        {
            var s = configRepository.GetValue(ConfigType.SystemVersion, "service.version");
            Version ver = null;

            if (!Version.TryParse(s, out ver) || ver < this.version)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 设置系统初始化状态
        /// </summary>
        /// <param name="val"></param>
        private void SetInitSystemData()
        {
            configRepository.SetValue(ConfigType.SystemVersion, "service.version", this.version.ToString());
        }
        
    }
}
