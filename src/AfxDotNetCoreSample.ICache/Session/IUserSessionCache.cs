using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.ICache
{
    public interface IUserSessionCache : IBaseCache
    {
        UserSessionDto Get(string sid);

        void Set(string sid, UserSessionDto value);

        void Remove(string sid);

        void Expire(string sid, TimeSpan? expireIn);

        void Expire(string sid);
    }
}
