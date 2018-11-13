using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IService
{
    public interface IUserSessionService : IBaseService
    {
        UserSessionDto Get(string sid);

        void Set(string sid, UserSessionDto vm);

        void Expire(string sid);

        void Logout(string sid);
    }
}
