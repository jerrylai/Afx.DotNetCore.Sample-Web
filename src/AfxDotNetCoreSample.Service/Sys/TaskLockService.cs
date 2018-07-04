using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Common;

namespace AfxDotNetCoreSample.Service
{
    public class TaskLockService : BaseService, ITaskLockService
    {
        /// <summary>
        /// 尝试获取锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key， 空字符或null需要换成*</param>
        /// <param name="owner">锁定定者， 空字符或null需要换成*</param>
        /// <param name="timeout">锁定之后，释放超时，单位秒</param>
        /// <returns></returns>
        public virtual bool Lock(TaskLockType type, string key, string owner, TimeSpan? timeout)
        {
            var repository = this.GetRepository<ITaskLockRepository>();
            bool result = repository.Lock(type, key, owner, timeout);

            return result;
        }

        /// <summary>
        /// 查询是否锁定
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key， 空字符或null需要换成*</param>
        /// <returns></returns>
        public virtual bool IsLock(TaskLockType type, string key)
        {
            var repository = this.GetRepository<ITaskLockRepository>();
            bool result = repository.IsLock(type, key);

            return result;
        }

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key， 空字符或null需要换成*</param>
        public virtual void Release(TaskLockType type, string key)
        {
            var repository = this.GetRepository<ITaskLockRepository>();
            repository.Release(type, key);
        }
    }
}
