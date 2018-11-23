using System;
using System.Collections.Generic;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.IRepository;

namespace AfxDotNetCoreSample.Service
{
    public class UserSessionService : BaseService, IUserSessionService
    {
        protected virtual IUserSessionRepository repository => this.GetRepository<IUserSessionRepository>();


        public virtual UserSessionDto Get(string sid)
        {
            if (string.IsNullOrEmpty(sid)) throw new ApiParamNullException("sid");
            var vm = repository.Get(sid);

            return vm;
        }

        public virtual void Set(string sid, UserSessionDto vm)
        {
            if (string.IsNullOrEmpty(sid)) throw new ApiParamNullException("sid");
            if (vm == null)
            {
                repository.Remove(sid);
            }
            else
            {
                vm.Sid = sid;
                repository.Set(sid, vm);
            }
        }

        public virtual void Expire(string sid)
        {
            if (string.IsNullOrEmpty(sid)) throw new ApiParamNullException("sid");
            repository.Expire(sid);
        }

        public virtual void Logout(string sid)
        {
            if (string.IsNullOrEmpty(sid)) throw new ApiParamNullException("sid");
            repository.Remove(sid);
        }
    }
}
