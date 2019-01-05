using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.IService
{
    public interface IBaseService : IDisposable
    {
        void SetCurrentUser(UserSessionDto user);
    }
}
