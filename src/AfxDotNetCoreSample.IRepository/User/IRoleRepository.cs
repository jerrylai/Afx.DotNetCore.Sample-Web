using System;
using System.Collections.Generic;
using System.Text;
using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IRepository
{
    public interface IRoleRepository : IBaseRepository
    {
        RoleDto Get(string id);

        int Update(RoleDto vm);

        int Add(RoleDto vm);

        int Delete(string id);

        PageDataOutputDto<RoleDto> GetPage(RolePageInputDto vm);
    }
}
