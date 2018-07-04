﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.IService;

namespace AfxDotNetCoreSample.Service
{
    public abstract class BaseService : IBaseService
    {
        protected virtual T GetRepository<T>() where T : IBaseRepository => IocUtils.Get<T>();

        protected virtual T GetRepository<T>(string name) where T : IBaseRepository => IocUtils.Get<T>(name);

        protected virtual T GetRepository<T>(object[] args) where T : IBaseRepository => IocUtils.Get<T>(args);

        protected virtual T GetRepository<T>(string name, object[] args) where T : IBaseRepository => IocUtils.Get<T>(name, args);
    }
}
