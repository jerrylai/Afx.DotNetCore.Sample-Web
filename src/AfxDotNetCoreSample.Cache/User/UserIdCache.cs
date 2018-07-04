using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Cache
{
    public class UserIdCache : DataDbCache, IUserIdCache
    {
        public virtual string Get(string account)
        {
            if (string.IsNullOrEmpty(account)) return null;
            return base.Get<string>(account.ToLower());
        }

        public virtual void Remove(string account)
        {
            if (!string.IsNullOrEmpty(account)) base.Remove(account.ToLower());
        }

        public virtual void Set(string account, string id)
        {
            if (!string.IsNullOrEmpty(account)) base.Set(id, account.ToLower());
        }
    }
}
