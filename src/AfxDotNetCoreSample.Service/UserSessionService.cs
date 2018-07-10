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
        public virtual UserSessionDto Get()
        {
            string sid = SessionUtils.Sid;
            if (string.IsNullOrEmpty(sid)) throw new ArgumentNullException("sid");
            var repository = this.GetRepository<IUserSessionRepository>();
            var vm = repository.Get(sid);

            return vm;
        }

        public virtual void Set(UserSessionDto vm)
        {
            string sid = SessionUtils.Sid;
            if (string.IsNullOrEmpty(sid)) throw new ArgumentNullException("sid");
            var repository = this.GetRepository<IUserSessionRepository>();
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

        public virtual void Expire()
        {
            string sid = SessionUtils.Sid;
            if (string.IsNullOrEmpty(sid)) throw new ArgumentNullException("sid");
            var repository = this.GetRepository<IUserSessionRepository>();
            repository.Expire(sid);
        }

        public virtual void Logout()
        {
            string sid = SessionUtils.Sid;
            if (string.IsNullOrEmpty(sid)) throw new ArgumentNullException("sid");
            var repository = this.GetRepository<IUserSessionRepository>();
            repository.Remove(sid);
            SessionUtils.RestSid();
        }
    }
}
