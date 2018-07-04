using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class SessionDataCache : SessionDbCache, ISessionDataCache
    {
        public virtual T Get<T>(string sid)
        {
            T value = default(T);
            if(!string.IsNullOrEmpty(sid))
            {
                value = base.Get<T>(sid);
            }

            return value;
        }

        public virtual void Remove(string sid)
        {
            if (!string.IsNullOrEmpty(sid))
            {
                 base.Remove(sid);
            }
        }
       
        public virtual void Set<T>(string sid, T value)
        {
            if (!string.IsNullOrEmpty(sid))
            {
                if (value != null) base.Set(value, sid);
                else base.Remove(sid);
            }
        }

        public virtual void Set<T>(string sid, T value, TimeSpan? expireIn)
        {
            if (!string.IsNullOrEmpty(sid))
            {
                if (value != null) base.Set(value, sid, expireIn);
                else base.Remove(sid);
            }
        }

        public virtual void Expire(string sid, TimeSpan? expireIn)
        {
            if (!string.IsNullOrEmpty(sid))
            {
                base.Expire(expireIn, sid);
            }
        }

        public void Expire(string sid)
        {
            if (!string.IsNullOrEmpty(sid))
            {
                var expire = base.GetConfigExpire();
                if (expire.HasValue) base.Expire(expire, sid);
            }
        }
    }
}
