using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class SessionCache : SessionDbCache, ISessionCache
    {
        public virtual void Expire(string sid, TimeSpan? expireIn)
        {
            if(!string.IsNullOrEmpty(sid))
            {
                base.Expire(expireIn, sid);
            }
        }

        public virtual T Get<T>(string sid)
        {
            T value = default(T);
            if (!string.IsNullOrEmpty(sid))
            {
                value = base.Get<T>(sid);
            }

            return value;
        }

        public virtual void Set<T>(string sid, T value)
        {
            if (!string.IsNullOrEmpty(sid))
            {
                base.Set(value, sid);
            }
        }

        public virtual void Remove(string sid)
        {
            if (!string.IsNullOrEmpty(sid))
            {
                base.Remove(sid);
            }
        }

        public virtual void Expire(string sid)
        {
            if (!string.IsNullOrEmpty(sid))
            {
                var expire = base.GetConfigExpire();
                if (expire.HasValue) base.Expire(expire, sid);
            }
        }
    }
}
