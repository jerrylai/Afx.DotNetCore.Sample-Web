using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afx.Cache;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public abstract class BaseCache<TCache> : IBaseCache
    {
        private readonly Lazy<Afx.Cache.ICache> _cache = new Lazy<Afx.Cache.ICache>(() => IocUtils.GetByKey<Afx.Cache.ICache>(ConfigUtils.CacheType));
        protected virtual Afx.Cache.ICache Cache => _cache.Value;

        private Type targetType;

        public Type TargetType => this.targetType;

        protected BaseCache(string node)
        {
            if (string.IsNullOrEmpty(node))
            {
                throw new ArgumentNullException("node");
            }
            this.Node = node;
            this.targetType = typeof(TCache);
            var name = this.TargetType.Name;
            if (this.targetType.IsInterface && name.StartsWith("I")) name = name.Substring(1);
            int index = name.LastIndexOf("Cache");
            if (index <= 0) throw new ArgumentException("TCache (" + this.TargetType.FullName + ") 错误！");
            this.Item = name.Substring(0, index);
        }

        

        protected virtual string GetCacheKey(params object[] args)
        {
            string key = CacheKeyUtils.GetKey(this.Node, this.Item);

            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == null) args[i] = "null";
                }
                key = string.Format(key, args);
            }

            return key;
        }

        private List<int> dbList = null;
        protected virtual List<int> GetCacheDb()
        {
            if (dbList == null)
            {
                dbList = CacheKeyUtils.GetDb(this.Node, this.Item);
                if (dbList.Count == 0) dbList.Add(0);
            }

            return dbList;
        }

        protected virtual int GetCacheDb(string key)
        {
            var list = this.GetCacheDb();
            if (list.Count < 2) return list.FirstOrDefault();
            var hash = key.GetHashCode();
            var db = list[hash % list.Count];

            return db;
        }

        protected virtual string Node { get; private set; }

        protected virtual string Item { get; private set; }

        public virtual TimeSpan? GetConfigExpire()
        {
            return CacheKeyUtils.GetExpire(this.Node, this.Item);
        }

        protected virtual T GetData<T>(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);

            return this.Cache.Get<T>(db, key);
        }

        protected virtual void SetData<T>(T value, params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var expireIn = this.GetConfigExpire();
            if (value == null) this.Cache.Remove(db, key);
            else this.Cache.Set<T>(db, key, value, expireIn);
        }

        protected virtual void SetData<T>(T value, TimeSpan? expireIn, params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            if (value == null) this.Cache.Remove(db, key);
            else this.Cache.Set<T>(db, key, value, expireIn);
        }

        protected virtual bool ContainsKey(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);

            return this.Cache.ContainsKey(db, key);
        }

        protected virtual void SetDataExpire(TimeSpan? expireIn, params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);

            this.Cache.Expire(db, key, expireIn);
        }

        protected virtual void RemoveKey(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);

            this.Cache.Remove(db, key);
        }

        protected virtual void RemoveDB()
        {
            var list = this.GetCacheDb();

            foreach (var db in list)
                this.Cache.FlushDb(db);
        }

        protected virtual void RemoveAllDB()
        {
            this.Cache.FlushAll();
        }
    }
}
