using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.ICache
{
    public interface IUserIdCache : IBaseCache
    {
        string Get(string account);
        void Set(string account, string id);
        void Remove(string account);
    }
}
