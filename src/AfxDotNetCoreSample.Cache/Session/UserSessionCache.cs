using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class UserSessionCache : SessionDbCache, IUserSessionCache
    {
        public virtual void Expire(string sid, TimeSpan? expireIn)
        {
            if(!string.IsNullOrEmpty(sid))
            {
                base.Expire(expireIn, sid);
            }
        }

        public virtual UserSessionDto Get(string sid)
        {
            UserSessionDto value = null;
            if (!string.IsNullOrEmpty(sid))
            {
                value = base.Get<UserSessionDto>(sid);
            }

            return value;
        }

        public virtual void Set(string sid, UserSessionDto value)
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
