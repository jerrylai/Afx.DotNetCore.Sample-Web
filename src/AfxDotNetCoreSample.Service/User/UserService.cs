using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.Common;

namespace AfxDotNetCoreSample.Service
{
    public class UserService : BaseService, IUserService
    {
        public virtual LoginInfoDto Login(LoginParamDto vm)
        {
            LoginInfoDto result = null;
            var userRepository = this.GetRepository<IUserRepository>();
            var userId = userRepository.GetId(vm.Account);
            if(!string.IsNullOrEmpty(userId))
            {
                
                var userDto = userRepository.Get(userId);
                if(userDto.Account == "admin" && string.IsNullOrEmpty(userDto.Password))
                {
                    userDto.Password = EncryptUtils.Md5("admin");
                    userRepository.UpdatePassword(userDto.Id, userDto.Password);
                }

                if (userDto.Status == UserStatus.Enabled)
                {
                    if (!string.IsNullOrEmpty(userDto.Password))
                    {
                        var pwd = EncryptUtils.Md5($"{userDto.Password}|{vm.Random}");
                        if (string.Equals(vm.Password, pwd, StringComparison.OrdinalIgnoreCase))
                        {
                            result = new LoginInfoDto()
                            {
                                Id = userDto.Id,
                                RoleId = userDto.RoleId,
                                Account = userDto.Account,
                                Name = userDto.Name
                            };
                        }
                    }
                }
                else
                {
                    LogUtils.Info($"【登录】{userDto.Name}({userDto.Account}) 已被禁用，登录失败！");
                    throw new ApiException("账号已被禁用！");
                }
            }

            return result;
        }
        
        public virtual UserDto Get(string id)
        {
            var userRepository = this.GetRepository<IUserRepository>();
            var m = userRepository.Get(id);
            if (m != null) m.Password = null;

            return m;
        }

        public virtual bool IsSetPwd(string id)
        {
            bool result = false;
            var userRepository = this.GetRepository<IUserRepository>();
            var m = userRepository.Get(id);
            if (m != null && !string.IsNullOrEmpty(m.Password))
                result = true;

            return result;
        }

        public virtual bool EditPwd(UserEditPwdDto vm)
        {
            bool result = false;
            var userRepository = this.GetRepository<IUserRepository>();
            var m = userRepository.Get(vm.UserId);
            if(m != null && !string.IsNullOrEmpty(m.Password))
            {
                var old = EncryptUtils.Decrypt(m.Password);
                if(old == vm.OldPwd)
                {
                    userRepository.UpdatePassword(m.Id, EncryptUtils.Encrypt(vm.NewPwd));
                    result = true;
                }
            }

            return result;
        }
    }
}
