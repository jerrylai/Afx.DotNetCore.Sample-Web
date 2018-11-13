using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IRepository
{
    public interface IUserRepository : IBaseRepository
    {
        UserDto Get(string id);

        string GetId(string account);

        string GetIdByMobile(string mobile);

        string GetIdByMail(string mail);

        int Update(UserDto vm);

        int UpdatePassword(string id, string pwd);

        int Add(UserDto vm);

        int Delete(string id);

        PageDataOutputDto<UserDto> GetPageData(UserPageInputDto vm);

        int GetUserCount(string roleId);
    }
}
