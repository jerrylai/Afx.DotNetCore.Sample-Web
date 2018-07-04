using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class UserCache : DataDbCache, IUserCache
    {
        public virtual UserDto Get(string id)
        {
            return base.Get<UserDto>(id);
        }

        public virtual void Remove(string id)
        {
            base.Remove(id);
        }

        public virtual void Set(string id, UserDto m)
        {
            base.Set(m, id);
        }
    }
}
