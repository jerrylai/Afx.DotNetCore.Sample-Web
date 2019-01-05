using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.IService;

namespace AfxDotNetCoreSample.Service
{
    public abstract class BaseService : IBaseService
    {
        public virtual UserSessionDto CurrentUser { get; private set; }

        public virtual void SetCurrentUser(UserSessionDto user)
        {
            this.CurrentUser = user;
        }

        private Dictionary<Type, IBaseRepository> repositoryDic = new Dictionary<Type, IBaseRepository>(5);
        protected virtual T GetRepository<T>(string name, object[] args) where T : IBaseRepository
        {
            var type = typeof(T);
            IBaseRepository repository = null;
            if (!repositoryDic.TryGetValue(type, out repository))
            {
                repositoryDic[type] = repository = IocUtils.Get<T>(name, args);
            }

            return (T)repository;
        }

        protected virtual T GetRepository<T>(string name) where T : IBaseRepository
        {
            return this.GetRepository<T>(name, null);
        }

        protected virtual T GetRepository<T>(object[] args) where T : IBaseRepository
        {
            return this.GetRepository<T>(null, args);
        }

        protected virtual T GetRepository<T>() where T : IBaseRepository
        {
            return this.GetRepository<T>(null, null);
        }

        private Dictionary<Type, IBaseService> serviceDic = new Dictionary<Type, IBaseService>(5);
        protected virtual T GetService<T>(string name, object[] args) where T : IBaseService
        {
            var type = typeof(T);
            IBaseService service = null;
            if (!serviceDic.TryGetValue(type, out service))
            {
                serviceDic[type] = service = IocUtils.Get<T>(name, args);
                service.SetCurrentUser(this.CurrentUser);
            }

            return (T)service;
        }

        protected virtual T GetService<T>(string name) where T : IBaseService
        {
            return this.GetService<T>(name, null);
        }

        protected virtual T GetService<T>(object[] args) where T : IBaseService
        {
            return this.GetService<T>(null, args);
        }

        protected virtual T GetService<T>() where T : IBaseService
        {
            return this.GetService<T>(null, null);
        }

        public virtual void Dispose()
        {
            if (this.serviceDic != null)
            {
                foreach (var kv in this.serviceDic)
                {
                    kv.Value.Dispose();
                }
                this.serviceDic.Clear();
                this.serviceDic = null;
            }

            if (this.repositoryDic != null)
            {
                foreach (var kv in this.repositoryDic)
                {
                    kv.Value.Dispose();
                }
                this.repositoryDic.Clear();
                this.repositoryDic = null;
            }
        }
    }
}
