using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfxDotNetCoreSample.Cache
{
   public abstract  class SessionDbCache<TCache> :  BaseCache<TCache>
    {
        public SessionDbCache() : base("SessionDb") { }

        protected override void SetDataExpire(TimeSpan? expireIn, params object[] args)
        {
            if (args == null || args.Length < 1) throw new ArgumentNullException("args");
            string key = base.GetCacheKey(args);
            int db = base.GetCacheDb(args[0].ToString());

            base.Cache.Expire(db, key, expireIn);
        }

        protected override T GetData<T>(params object[] args)
        {
            if (args == null || args.Length < 1) throw new ArgumentNullException("args");
            string key = base.GetCacheKey(args);
            int db = base.GetCacheDb(args[0].ToString());

            return base.Cache.Get<T>(db, key);
        }

        protected override void SetData<T>(T value, params object[] args)
        {
            if (args == null || args.Length < 1) throw new ArgumentNullException("args");
            string key = base.GetCacheKey(args);
            int db = base.GetCacheDb(args[0].ToString());
            var expireIn = base.GetConfigExpire();
            if (value == null) base.Cache.Remove(db, key);
            else base.Cache.Set<T>(db, key, value, expireIn);
        }

        protected override void SetData<T>(T value, TimeSpan? expireIn, params object[] args)
        {
            if (args == null || args.Length < 1) throw new ArgumentNullException("args");
            string key = base.GetCacheKey(args);
            int db = base.GetCacheDb(args[0].ToString());
            if (value == null) base.Cache.Remove(db, key);
            else base.Cache.Set<T>(db, key, value, expireIn);
        }

        protected override bool ContainsKey(params object[] args)
        {
            if (args == null || args.Length < 1) throw new ArgumentNullException("args");
            string key = base.GetCacheKey(args);
            int db = base.GetCacheDb(args[0].ToString());

            return base.Cache.ContainsKey(db, key);
        }
        
        protected override void RemoveKey(params object[] args)
        {
            if (args == null || args.Length < 1) throw new ArgumentNullException("args");
            string key = base.GetCacheKey(args);
            int db = base.GetCacheDb(args[0].ToString());

            base.Cache.Remove(db, key);
        }
    }
}
