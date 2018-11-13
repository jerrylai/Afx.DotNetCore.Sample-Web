using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.IRepository;
using AfxDotNetCoreSample.Enums;
using AfxDotNetCoreSample.Common;
using Afx.Utils;

namespace AfxDotNetCoreSample.Service
{
    public class UserService : BaseService, IUserService
    {
        private readonly Lazy<IUserRepository> _userRepository = new Lazy<IUserRepository>(() => IocUtils.Get<IUserRepository>());
        internal protected virtual IUserRepository userRepository => this._userRepository.Value;

        private Lazy<IRoleRepository> _roleRepository = new Lazy<IRoleRepository>(IocUtils.Get<IRoleRepository>);
        private IRoleRepository roleRepository => this._roleRepository.Value;

        public virtual string GetUserIdForValue(string value)
        {
            string id = null;
            if (!string.IsNullOrEmpty(value))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(value, "^[1][3,4,5,7,8,9][0-9]{9}$"))
                {
                    id = this.userRepository.GetIdByMobile(value);
                }
                else if (System.Text.RegularExpressions.Regex.IsMatch(value, "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$"))
                {
                    id = userRepository.GetIdByMail(value);
                }
                else
                {
                    id = userRepository.GetId(value);
                }
            }
            

            return id;
        }

        public virtual LoginOutputDto Login(LoginInputDto vm)
        {
            LoginOutputDto result = null;
            var userId = this.GetUserIdForValue(vm.Account);
            if (!string.IsNullOrEmpty(userId))
            {

                var userDto = userRepository.Get(userId);
                if (userDto.Account == "admin" && string.IsNullOrEmpty(userDto.Password))
                {
                    userDto.Password = EncryptUtils.Encrypt("admin");
                    userRepository.UpdatePassword(userDto.Id, userDto.Password);
                }

                if (userDto.Status == UserStatus.Disabled)
                {
                    LogUtils.Info($"【登录】{userDto.Name}({userDto.Account}) 已被禁用，登录失败！");
                    throw new ApiException(ApiStatus.Error, "账号已被禁用, 请与管理员联系！");
                }
                if (!string.IsNullOrEmpty(userDto.Password))
                {
                    var pwd = EncryptUtils.Decrypt(userDto.Password);
                    pwd = EncryptUtils.Md5(pwd);
                    pwd = EncryptUtils.Md5($"{pwd}|{vm.Random}");
                    if (string.Equals(vm.Password, pwd, StringComparison.OrdinalIgnoreCase))
                    {
                        result = new LoginOutputDto()
                        {
                            Id = userDto.Id,
                            RoleId = userDto.RoleId,
                            Account = userDto.Account,
                            Name = userDto.Name
                        };
                    }
                }
            }

            return result;
        }
        
        public virtual UserDto Get(string id)
        {
            var m = userRepository.Get(id);
            if (m != null) m.Password = null;

            return m;
        }

        public virtual bool IsSetPwd(string id)
        {
            bool result = false;
            var m = userRepository.Get(id);
            if (m != null && !string.IsNullOrEmpty(m.Password))
                result = true;

            return result;
        }

        public virtual bool EditPwd(UserEditPwdInputDto vm)
        {
            bool result = false;
            var m = userRepository.Get(vm.UserId);
            if(m != null && !string.IsNullOrEmpty(m.Password))
            {
                var old = EncryptUtils.Decrypt(m.Password);
                if(old == vm.OldPwd)
                {
                    userRepository.UpdatePassword(m.Id, EncryptUtils.Encrypt(vm.NewPwd));
                    result = true;
                }
                else
                {
                    throw new ApiException("旧密码不正确！");
                }
            }

            return result;
        }

        public virtual PageDataOutputDto<UserDto> GetPageData(UserPageInputDto vm)
        {
            if (vm == null) throw new ApiParamNullException(nameof(vm));
            if (vm.PageIndex < 1) throw new ApiParamException(nameof(vm.PageIndex));
            if (vm.PageSize < 1) throw new ApiParamException(nameof(vm.PageSize));
            var pagedata = this.userRepository.GetPageData(vm);
            List<RoleDto> rolelsit = new List<RoleDto>();
            foreach(var m in pagedata.Data)
            {
                var role = rolelsit.Find(q => q.Id == m.RoleId);
                if (role == null) role = roleRepository.Get(m.RoleId);
                m.RoleName = role?.Name;
            }

            return pagedata;
        }

        public virtual bool Add(UserDto vm)
        {
            bool result = false;
            if (vm == null) throw new ApiParamNullException(nameof(vm));
            if(string.IsNullOrEmpty(vm.Account)) throw new ApiParamNullException(nameof(vm.Account));
            if (string.IsNullOrEmpty(vm.Name)) throw new ApiParamNullException(nameof(vm.Name));
            //if (string.IsNullOrEmpty(vm.Password)) throw new ApiArgumentNullException(nameof(vm.Password));
            if (string.IsNullOrEmpty(vm.RoleId)) throw new ApiParamNullException(nameof(vm.RoleId));
            var role = this.roleRepository.Get(vm.RoleId);
            if(role == null) throw new ApiParamException(nameof(vm.RoleId));
            if(!string.IsNullOrEmpty(vm.Password)) vm.Password = EncryptUtils.Encrypt(vm.Password);
            using(var syncLock = IocUtils.Get<ISyncLock>())
            {
                syncLock.Init(LockType.AddUserAccount, vm.Account.ToLower(), null, TimeSpan.FromMinutes(3));
                if (syncLock.Lock())
                {
                    var id = this.userRepository.GetId(vm.Account);
                    if(!string.IsNullOrEmpty(id)) throw new ApiException("账号已存在！");
                    using (var syncLockMobile = IocUtils.Get<ISyncLock>())
                    {
                        if (!string.IsNullOrEmpty(vm.Mobile))
                        {
                            syncLockMobile.Init(LockType.AddUserMobile, vm.Mobile, null, TimeSpan.FromMinutes(3));
                            if (syncLockMobile.Lock())
                            {
                                id = this.userRepository.GetIdByMobile(vm.Mobile);
                                if (!string.IsNullOrEmpty(id)) throw new ApiException("手机号码已存在！");
                            }
                            else
                            {
                                throw new ApiException("系统正忙，请稍后再试！");
                            }
                        }

                        using (var syncLockMail = IocUtils.Get<ISyncLock>())
                        {
                            if (!string.IsNullOrEmpty(vm.Mail))
                            {
                                syncLockMail.Init(LockType.AddUserMail, vm.Mail, null, TimeSpan.FromMinutes(3));
                                if (syncLockMail.Lock())
                                {
                                    id = this.userRepository.GetIdByMail(vm.Mail);
                                    if (!string.IsNullOrEmpty(id)) throw new ApiException("邮箱已存在！");
                                }
                                else
                                {
                                    throw new ApiException("系统正忙，请稍后再试！");
                                }
                            }
                            var count = this.userRepository.Add(vm);
                            result = true;
                        }
                    }
                }
                else
                {
                    throw new ApiException("系统正忙，请稍后再试！");
                }
            }

            return result;
        }

        public virtual bool Update(UserDto vm)
        {
            bool result = false;
            if (vm == null) throw new ApiParamNullException(nameof(vm));
            if (string.IsNullOrEmpty(vm.Id)) throw new ApiParamNullException(nameof(vm.Id));
            if (string.IsNullOrEmpty(vm.Name)) throw new ApiParamNullException(nameof(vm.Name));
            if (string.IsNullOrEmpty(vm.RoleId)) throw new ApiParamNullException(nameof(vm.RoleId));
            var m = this.userRepository.Get(vm.Id);
            if (m == null) throw new ApiParamException(nameof(vm.Id));
            //if(m.IsSystem == true) throw new ApiException("系统默认数据不能修改！");
            var role = this.roleRepository.Get(vm.RoleId);
            if (role == null) throw new ApiParamException(nameof(vm.RoleId));
            using (var syncLockMobile = IocUtils.Get<ISyncLock>())
            {
                if (!string.IsNullOrEmpty(vm.Mobile))
                {
                    syncLockMobile.Init(LockType.AddUserMobile, vm.Mobile, null, TimeSpan.FromMinutes(3));
                    if (syncLockMobile.Lock())
                    {
                       var id = this.userRepository.GetIdByMobile(vm.Mobile);
                        if (!string.IsNullOrEmpty(id) && id != vm.Id) throw new ApiException("手机号码已存在！");
                    }
                    else
                    {
                        throw new ApiException("系统正忙，请稍后再试！");
                    }
                }

                using (var syncLockMail = IocUtils.Get<ISyncLock>())
                {
                    if (!string.IsNullOrEmpty(vm.Mail))
                    {
                        syncLockMail.Init(LockType.AddUserMail, vm.Mail, null, TimeSpan.FromMinutes(3));
                        if (syncLockMail.Lock())
                        {
                           var id = this.userRepository.GetIdByMail(vm.Mail);
                            if (!string.IsNullOrEmpty(id) && id != vm.Id) throw new ApiException("邮箱已存在！");
                        }
                        else
                        {
                            throw new ApiException("系统正忙，请稍后再试！");
                        }
                    }
                    if (!string.IsNullOrEmpty(vm.Password)) vm.Password = EncryptUtils.Encrypt(vm.Password);
                    var count = this.userRepository.Update(vm);
                    result = true;
                }
            }

            return result;
        }

        public virtual bool Delete(string id)
        {
            bool result = false;
            if (string.IsNullOrEmpty(id)) throw new ApiParamNullException(nameof(id));
            var m = this.userRepository.Get(id);
            if (m == null) throw new ApiParamException(nameof(id));
            if(m.IsSystem == true) throw new ApiException("系统默认数据不能删除！");
            var count = this.userRepository.Delete(id);
            result = true;

            return result;
        }

        public virtual string GetId(string account)
        {
            if (string.IsNullOrEmpty(account)) throw new ApiParamNullException(nameof(account));
            string id = this.userRepository.GetId(account);

            return id;
        }

        public virtual string GetIdByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) throw new ApiParamNullException(nameof(mobile));
            string id = this.userRepository.GetIdByMobile(mobile);

            return id;
        }

        public virtual string GetIdByMail(string mail)
        {
            if (string.IsNullOrEmpty(mail)) throw new ApiParamNullException(nameof(mail));
            string id = this.userRepository.GetIdByMail(mail);

            return id;
        }
    }
}
