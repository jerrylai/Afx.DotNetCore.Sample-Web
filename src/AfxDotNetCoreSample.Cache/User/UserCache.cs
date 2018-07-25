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
            return base.GetData<UserDto>(id);
        }

        public virtual void Remove(string id)
        {
            base.RemoveKey(id);
        }

        public virtual void Set(string id, UserDto m)
        {
            base.SetData(m, id);
        }
    }
}
