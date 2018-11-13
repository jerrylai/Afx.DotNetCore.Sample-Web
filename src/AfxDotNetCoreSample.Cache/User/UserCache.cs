using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class UserCache : DataDbCache<UserCache>, IUserCache
    {
        public virtual UserDto Get(string id)
        {
            return this.GetData<UserDto>(id);
        }

        public virtual void Remove(string id)
        {
            this.RemoveKey(id);
        }

        public virtual void Set(string id, UserDto m)
        {
            this.SetData(m, id);
        }
    }
}
