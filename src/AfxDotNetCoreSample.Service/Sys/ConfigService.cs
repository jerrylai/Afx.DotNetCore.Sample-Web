using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Common;
using System.Reflection;

namespace AfxDotNetCoreSample.Service
{
    /// <summary>
    /// 系统配置业务实现
    /// </summary>
    public class ConfigService : BaseService, IConfigService
    {
        protected virtual IConfigRepository repository => this.GetRepository<IConfigRepository>();
        
        /// <summary>
        /// 根据配置类型获取
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <returns></returns>
        public virtual Dictionary<string, string> GetDic(ConfigType type)
        {
            return this.repository.Get(type);
        }

        /// <summary>
        /// 更新系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <param name="dic">系统配置集合</param>
        /// <returns></returns>
        public virtual bool SetDic(ConfigType type, Dictionary<string, string> dic)
        {
            if (dic == null) throw new ApiException(ApiStatus.Error, "dic is null!");
            this.repository.Set(type, dic);
            return true;
        }

        /// <summary>
        /// 获取单个系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <returns></returns>
        public virtual string GetValue(ConfigType type)
        {
            return this.repository.GetValue(type);
        }

        /// <summary>
        /// 获取单个系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <param name="name">名称, 空字符串或null需要改成*</param>
        /// <returns></returns>
        public virtual string GetValue(ConfigType type, string name)
        {

            return this.repository.GetValue(type, name);
        }

        /// <summary>
        /// 更新系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool SetValue(ConfigType type, string value)
        {
            this.repository.SetValue(type, value);
            return true;
        }

        /// <summary>
        /// 更新系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <param name="name">名称, 空字符串或null需要改成*</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool SetValue(ConfigType type, string name, string value)
        {
            this.repository.SetValue(type, name, value);
            return true;
        }

        public virtual DateTime GetLocalNow()
        {
            return this.repository.GetLocalNow();
        }

        public virtual DateTime GetUtcNow()
        {
            return this.repository.GetUtcNow();
        }

        public virtual T GetModel<T>(ConfigType type) where T : class, new()
        {
            var t = typeof(T);
            var arr = t.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Instance);
            Dictionary<string, string> dic = this.repository.Get(type);
            var m = new T();
            foreach(var p in arr)
            {
                string v = null;
                if(dic.TryGetValue(p.Name, out v) && !string.IsNullOrEmpty(v))
                {
                    var val = Convert.ChangeType(v, p.PropertyType);
                    p.SetValue(m, val);
                }
            }

            return m;
        }

        public virtual bool SetModel<T>(ConfigType type, T m) where T : class, new()
        {
            if (m == null) throw new ApiParamNullException(nameof(m));
            var t = typeof(T);
            var arr = t.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Instance);
            Dictionary<string, string> dic = new Dictionary<string, string>(arr.Length);
            foreach (var p in arr)
            {
                dic[p.Name] = p.GetValue(m)?.ToString() ?? "";
            }

            var count = this.repository.Set(type, dic);

            return true;
        }
    }
}
