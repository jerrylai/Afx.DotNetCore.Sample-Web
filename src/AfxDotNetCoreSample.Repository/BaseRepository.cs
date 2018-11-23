﻿using System;
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

        private Dictionary<Type, IBaseCache> cacheDic = new Dictionary<Type, IBaseCache>(5);
        protected virtual T GetCache<T>(string name, object[] args) where T : IBaseCache
        {
            var type = typeof(T);
            IBaseCache cache = null;
            if (!cacheDic.TryGetValue(type, out cache))
            {
                cacheDic[type] = cache = IocUtils.Get<T>(name, args);
            }

            return (T)cache;
        }

        protected virtual T GetCache<T>(string name) where T : IBaseCache
        {
            return this.GetCache<T>(name, null);
        }

        protected virtual T GetCache<T>(object[] args) where T : IBaseCache
        {
            return this.GetCache<T>(null, args);
        }

        protected virtual T GetCache<T>() where T : IBaseCache
        {
            return this.GetCache<T>(null, null);
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

        public virtual void Dispose()
        {
            if (this.cacheDic != null)
            {
                foreach (var kv in this.cacheDic)
                {
                    kv.Value.Dispose();
                }
                this.cacheDic.Clear();
                this.cacheDic = null;
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
