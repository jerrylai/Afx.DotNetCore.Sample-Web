using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Common;

namespace AfxDotNetCoreSample.Repository
{
    public class UserSessionRepository : BaseRepository, IUserSessionRepository
    {
        protected virtual IUserSessionCache cache => this.GetCache<IUserSessionCache>();

        public virtual void Expire(string sid, TimeSpan? expireIn)
        {
            this.cache.Expire(sid, expireIn);
        }

        public virtual void Expire(string sid)
        {
            this.cache.Expire(sid);
        }

        public virtual UserSessionDto Get(string sid)
        {
            UserSessionDto value = this.cache.Get(sid);
            
            return value;
        }

        public virtual void Remove(string sid)
        {
            this.cache.Remove(sid);
        }

        public virtual void Set(string sid, UserSessionDto value)
        {
            this.cache.Set(sid, value);
        }
    }
}
