using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfxDotNetCoreSample.ICache
{
    public interface ISessionDataCache : IBaseCache
    {
        T Get<T>(string sid);

        void Set<T>(string sid, T value);

        void Set<T>(string sid, T value, TimeSpan? expireIn);

        void Remove(string sid);
        
        void Expire(string sid, TimeSpan? expireIn);

        void Expire(string sid);
    }
}
