using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using AfxDotNetCoreSample.Enums;

namespace AfxDotNetCoreSample.Dto
{
    public class UserDto
    {
        public string Id { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [MaxLength(100)]
        public string Account { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(100)]
        public string Password { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [MaxLength(11)]
        public string Mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(200)]
        public string Mail { get; set; }
        /// <summary>
        /// 状态枚举
        /// </summary>
        public UserStatus Status { get; set; }
        /// <summary>
        /// 0.用户添加；1.系统内置
        /// </summary>
        public bool IsSystem { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
