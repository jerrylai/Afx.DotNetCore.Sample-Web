using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IService
{
    public interface IUserService : IBaseService
    {
        string GetUserIdForValue(string value);

        LoginOutputDto Login(LoginInputDto vm);

        UserDto Get(string id);

        bool IsSetPwd(string id);

        bool EditPwd(UserEditPwdInputDto vm);

        PageDataOutputDto<UserDto> GetPageData(UserPageInputDto vm);

        bool Add(UserDto vm);

        bool Update(UserDto vm);

        bool Delete(string id);

        string GetId(string account);

        string GetIdByMobile(string mobile);

        string GetIdByMail(string mail);
    }
}
