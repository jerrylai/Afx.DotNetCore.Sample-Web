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
            var repository = this.GetRepository<ISystemRepository>();
            try { repository.InitDb(); }
            catch(Exception ex)
            {
                LogUtils.Info("【InitDatabase】", ex);
            }
            if (this.IsInitSystemData())
            {
                using (var locked = this.GetSyncLock(TaskLockType.InitSystemData, "0", null, TimeSpan.FromHours(1)))
                {
                    if (locked.Lock())
                    {
                        repository.InitData();
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
            var repository = this.GetRepository<IConfigRepository>();
            var s = repository.GetValue(ConfigType.SystemVersion, "*");
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
            var repository = GetRepository<IConfigRepository>();
            repository.SetValue(ConfigType.SystemVersion, "*", this.version.ToString());
        }
        
    }
}
