using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Models;
using AfxDotNetCoreSample.Common;
using System.Data;

namespace AfxDotNetCoreSample.Repository
{
    /// <summary>
    /// 系统配置实现
    /// </summary>
    public class ConfigRepository : BaseRepository, IConfigRepository
    {
        protected virtual IConfigCache cache => this.GetCache<IConfigCache>();

        /// <summary>
        /// 根据配置类型获取
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <returns></returns>
        public virtual Dictionary<string, string> Get(ConfigType type)
        {
            List<ConfigDto> list = null;
            list = this.cache.Get(type);
            if(list == null)
            {
                using (var db = this.GetContext())
                {
                    #region
                    var query = from q in db.SysConfig
                                where q.Type == type
                                select new ConfigDto
                                {
                                    Id = q.Id,
                                    Type = q.Type,
                                    Name = q.Name,
                                    Value = q.Value
                                };
                    #endregion

                    list = query.ToList();
                    this.cache.Set(type, list);
                }
            }
            Dictionary<string, string> dic = new Dictionary<string, string>(list.Count);
            foreach(var vm in list)
            {
                dic[vm.Name] = vm.Value;
            }

            return dic;
        }

        /// <summary>
        /// 更新系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <param name="dic">系统配置集合</param>
        /// <returns></returns>
        public virtual int Set(ConfigType type, Dictionary<string, string> dic)
        {
            int count = 0;
            using (var db = this.GetContext())
            {
                using (db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var list = db.SysConfig.Where(q => q.Type == type).ToList();
                    foreach (var kv in dic)
                    {
                        string key = string.IsNullOrEmpty(kv.Key) ? "*" : kv.Key;
                        SysConfig m = list.Find(q => q.Name == key);
                        if (m == null)
                        {
                            m = new SysConfig()
                            {
                                Id = this.GetIdentity<SysConfig>(),
                                Type = type,
                                Name = key
                            };
                            db.SysConfig.Add(m);
                        }
                        m.Value = kv.Value;
                    }

                    var dellist = list.FindAll(q => !dic.ContainsKey(q.Name));
                    foreach (var m in dellist)
                    {
                        db.SysConfig.Remove(m);
                    }

                    count = db.SaveChanges();
                    db.Commit();
                    if (count > 0)
                    {
                        this.cache.Remove(type);
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// 获取单个系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <returns></returns>
        public virtual string GetValue(ConfigType type)
        {
            return this.GetValue(type, "*");
        }

        /// <summary>
        /// 获取单个系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <param name="name">名称, 空字符串或null需要改成*</param>
        /// <returns></returns>
        public virtual string GetValue(ConfigType type, string name)
        {
            if (string.IsNullOrEmpty(name)) name = "*";
            string value = null;
            var dic = this.Get(type);
            dic.TryGetValue(name, out value);

            return value;
        }

        /// <summary>
        /// 更新系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual int SetValue(ConfigType type, string value)
        {
            return this.SetValue(type, "*", value);
        }

        /// <summary>
        /// 更新系统配置
        /// </summary>
        /// <param name="type">ConfigType枚举值</param>
        /// <param name="name">名称, 空字符串或null需要改成*</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual int SetValue(ConfigType type, string name, string value)
        {
            if (string.IsNullOrEmpty(name)) name = "*";
            Dictionary<string, string> dic = new Dictionary<string, string>() { { name, value } };

            return this.Set(type, dic);
        }

        public DateTime GetLocalNow()
        {
            using (var db = this.GetContext())
            {
                return db.GetLocalNow();
            }
        }

        public DateTime GetUtcNow()
        {
            using (var db = this.GetContext())
            {
                return db.GetUtcNow();
            }
        }
    }
}
