using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.ICache
{
    public interface IUserCache : IBaseCache
    {
        UserDto Get(string id);
        void Set(string id, UserDto m);
        void Remove(string id);
    }
}
