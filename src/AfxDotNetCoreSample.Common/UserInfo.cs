using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfxDotNetCoreSample.Common
{
    public class UserInfo
    {
        internal bool IsModify = false;
        
        private string userId;
        public string UserId
        {
            get { return userId; }
            set
            {
                if(userId != value)
                {
                    userId = value;
                    IsModify = true;
                }
            }
        }

        private string roleId;
        public string RoleId
        {
            get { return roleId; }
            set
            {
                if (roleId != value)
                {
                    roleId = value;
                    IsModify = true;
                }
            }
        }

        private string account;
        public string Account
        {
            get { return account; }
            set
            {
                if (account != value)
                {
                    account = value;
                    IsModify = true;
                }
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    IsModify = true;
                }
            }
        }

        private DateTime loginTime = DateTime.Now;
        public DateTime LoginTime
        {
            get { return loginTime; }
            set
            {
                if (loginTime != value)
                {
                    loginTime = value;
                    IsModify = true;
                }
            }
        }
    }
}
