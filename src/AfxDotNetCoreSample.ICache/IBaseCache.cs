using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfxDotNetCoreSample.ICache
{
    public interface IBaseCache
    {
        TimeSpan? GetConfigExpire();
    }
}
