using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IService
{
    public interface IUserService : IBaseService
    {
        LoginOutputDto Login(LoginInputDto vm);

        UserDto Get(string id);

        bool IsSetPwd(string id);

        bool EditPwd(UserEditPwdInputDto vm);
    }
}
