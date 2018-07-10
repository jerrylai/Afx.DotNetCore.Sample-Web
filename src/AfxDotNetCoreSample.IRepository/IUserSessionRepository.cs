using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IRepository
{
    public interface IUserSessionRepository : IBaseRepository
    {
        UserSessionDto Get(string sid);

        void Set(string sid, UserSessionDto value);

        void Remove(string sid);

        void Expire(string sid, TimeSpan? expireIn);

        void Expire(string sid);
    }
}
