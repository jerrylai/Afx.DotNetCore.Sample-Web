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
    public class DistributedLockService : BaseService, IDistributedLockService
    {
        protected virtual IDistributedLockRepository repository => this.GetRepository<IDistributedLockRepository>();

        private string FormatValue(string v, string name)
        {
            var s = v;
            if (v.Length > 50)
            {
                s = EncryptUtils.Md5(v);
                LogUtils.Info($"【TaskLockService.FormatValue】{name}.Length > 50 : {name}={v}, md5.{name}={s}");
            }

            return s;
        }

        /// <summary>
        /// 尝试获取锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空, key长度小于或等于50</param>
        /// <param name="owner">锁定定者，不能为空, owner长度小于或等于50</param>
        /// <param name="timeout">锁超时</param>
        /// <returns></returns>
        public virtual bool Lock(LockType type, string key, string owner, TimeSpan? timeout)
        {
            if (string.IsNullOrEmpty(key)) throw new ApiParamNullException(nameof(key));
            if (string.IsNullOrEmpty(owner)) throw new ApiParamNullException(nameof(owner));

            key = this.FormatValue(key, nameof(key));
            owner = this.FormatValue(owner, nameof(owner));

            return this.repository.Lock(type, key, owner, timeout);
        }

        /// <summary>
        /// 查询是否锁定
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空，key长度小于或等于50</param>
        /// <param name="owner">锁定定者，不能为空, owner长度小于或等于50</param>
        /// <returns></returns>
        public virtual bool IsOtherLock(LockType type, string key, string owner)
        {
            if (string.IsNullOrEmpty(key)) throw new ApiParamNullException(nameof(key));
            if (string.IsNullOrEmpty(owner)) throw new ApiParamNullException(nameof(owner));

            key = this.FormatValue(key, nameof(key));
            owner = this.FormatValue(owner, nameof(owner));

            return this.repository.IsOtherLock(type, key, owner);
        }

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空，key长度小于或等于50</param>
        public virtual void Release(LockType type, string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ApiParamNullException(nameof(key));

            key = this.FormatValue(key, nameof(key));

            this.repository.Release(type, key);
        }

        /// <summary>
        /// 更新Timeout
        /// </summary>
        /// <param name="type">锁类型</param>
        /// <param name="key">锁key，不能为空，key长度小于或等于50</param>
        /// <param name="owner">锁定定者，不能为空, owner长度小于或等于50</param>
        /// <param name="timeout">锁超时</param>
        /// <returns></returns>
        public virtual void UpdateTimeout(LockType type, string key, string owner, TimeSpan? timeout)
        {
            if (string.IsNullOrEmpty(key)) throw new ApiParamNullException(nameof(key));
            if (string.IsNullOrEmpty(owner)) throw new ApiParamNullException(nameof(owner));

            key = this.FormatValue(key, nameof(key));
            owner = this.FormatValue(owner, nameof(owner));

            this.repository.UpdateTimeout(type, key, owner, timeout);
        }
    }
}
