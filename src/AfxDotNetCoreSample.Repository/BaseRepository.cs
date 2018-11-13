using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Models;

namespace AfxDotNetCoreSample.Repository
{
    public abstract class BaseRepository: IBaseRepository
    {
        protected virtual AfxContext GetContext() => new AfxContext();

    }
}
