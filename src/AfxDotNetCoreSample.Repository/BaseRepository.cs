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
        protected virtual AfxDotNetCoreSampleContext GetContext() => new AfxDotNetCoreSampleContext();

        protected virtual T GetCache<T>() where T : IBaseCache => IocUtils.Get<T>();

        protected virtual T GetCache<T>(string name) where T : IBaseCache => IocUtils.Get<T>(name);

        protected virtual T GetCache<T>(object[] args) where T : IBaseCache => IocUtils.Get<T>(args);

        protected virtual T GetCache<T>(string name, object[] args) where T : IBaseCache => IocUtils.Get<T>(name, args);
    }
}
