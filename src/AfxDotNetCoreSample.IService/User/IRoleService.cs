using System;
using System.Collections.Generic;
using System.Text;
using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IService
{
    public interface IRoleService : IBaseService
    {
        RoleDto Get(string id);

        bool Update(RoleDto vm);

        bool Add(RoleDto vm);

        bool Delete(string id);

        PageDataOutputDto<RoleDto> GetPage(RolePageInputDto vm);
    }
}
