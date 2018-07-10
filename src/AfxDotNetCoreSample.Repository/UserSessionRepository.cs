using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;

namespace AfxDotNetCoreSample.Repository
{
    public class UserSessionRepository : BaseRepository, IUserSessionRepository
    {
        public virtual void Expire(string sid, TimeSpan? expireIn)
        {
            var cache = this.GetCache<IUserSessionCache>();
            cache.Expire(sid, expireIn);
        }

        public virtual void Expire(string sid)
        {
            var cache = this.GetCache<IUserSessionCache>();
            cache.Expire(sid);
        }

        public virtual UserSessionDto Get(string sid)
        {
            var cache = this.GetCache<IUserSessionCache>();

            return cache.Get(sid);
        }

        public virtual void Remove(string sid)
        {
            var cache = this.GetCache<IUserSessionCache>();
            cache.Remove(sid);
        }

        public virtual void Set(string sid, UserSessionDto value)
        {
            var cache = this.GetCache<IUserSessionCache>();
            cache.Set(sid, value);
        }
    }
}
