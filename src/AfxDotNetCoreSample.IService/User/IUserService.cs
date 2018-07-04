using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IService
{
    public interface IUserService : IBaseService
    {
        LoginInfoDto Login(LoginParamDto vm);

        UserDto Get(string id);

        bool IsSetPwd(string id);

        bool EditPwd(UserEditPwdDto vm);
    }
}
