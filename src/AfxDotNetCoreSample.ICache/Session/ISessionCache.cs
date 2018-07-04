using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.ICache
{
    public interface ISessionCache : IBaseCache
    {
        T Get<T>(string sid);

        void Set<T>(string sid, T value);

        void Remove(string sid);

        void Expire(string sid, TimeSpan? expireIn);

        void Expire(string sid);
    }
}
