using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IService
{
    public interface IUserSessionService : IBaseService
    {
        UserSessionDto Get();

        void Set(UserSessionDto vm);

        void Expire();

        void Logout();
    }
}
