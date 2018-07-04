using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Afx.Cache;
using Afx.Utils;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.ICache;

namespace AfxDotNetCoreSample.Common
{
    public static class SessionUtils
    {
        [ThreadStatic]
        private static UserSessionData userData;

        public static void OnRequestSid(string sid)
        {
            userData = new UserSessionData(sid);
        }

        public static string OnResponseSid(string sid)
        {
            var s = sid;
            if (userData != null)
            {
                userData.Expire();
                if (userData.IsRestSid) s = userData.Sid;
            }

            return s;
        }

        public static string Sid
        {
            get
            {
                return userData?.Sid;
            }
        }
        
        public static UserInfo User
        {
            get
            {
                return userData?.User;
            }
            set
            {
                var data = userData;
                if(data != null)
                {
                    data.User = value;
                }
            }
        }
        
        public static void Set<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            userData?.Set(key, value);
        }
        
        public static T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            T value = default(T);
            var o = userData?.Get(key);
            if (o != null) value = (T)o;

            return value;
        }

        public static void Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            userData?.Remove(key);
        }

        public static List<string> GetKeys()
        {
            return userData?.Data.Select(q => q.Key).ToList();
        }

        public static void Clear()
        {
            userData?.Clear();
        }

        public static void Logout()
        {
            userData?.Logout();
        }

        public class SessionData
        {
            public string Key { get; set; }

            public object Value { get; set; }
        }

        public class UserSessionData
        {
            public UserSessionData(string sid)
            {
                this.sid = sid;
            }

            private string sid;
            public string Sid
            {
                get
                {
                    var s = sid;
                    if (string.IsNullOrEmpty(s))
                    {
                        s = RestSid();
                    }

                    return s;
                }
            }

            public bool IsRestSid { get; private set; } = false;

            public string RestSid()
            {
                sid = Guid.NewGuid().ToString("n");
                this.IsRestSid = true;

                return sid;
            }

            private bool isModifyData = false;
            private List<SessionData> data;
            public List<SessionData> Data
            {
                get
                {
                    if(data == null)
                    {
                        if (!string.IsNullOrEmpty(sid))
                        {
                            var datacache = IocUtils.Get<ISessionDataCache>();
                            data = datacache.Get<List<SessionData>>(this.sid) ?? new List<SessionData>();
                        }
                        else
                        {
                            data = new List<SessionData>();
                        }
                    }

                    return data;
                }
            }

            private bool isLoadUser = false;
            private bool isModifyUser = false;
            private UserInfo user;
            public UserInfo User
            {
                get
                {
                    if(!this.isLoadUser && !this.isModifyUser)
                    {
                        this.isLoadUser = true;
                        if (!string.IsNullOrEmpty(this.sid))
                        {
                            var cache = IocUtils.Get<ISessionCache>();
                            this.user = cache.Get<UserInfo>(this.sid);
                            if (user != null) user.IsModify = false;
                        }
                    }

                    return user;
                }
                set
                {
                    if(this.user != value)
                    {
                        this.isModifyUser = true;
                        this.user = value;
                        if(this.user != null) user.IsModify = true;
                    }
                }
            }
            
            public object Get(string key)
            {
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
                return this.Data.Find(q => q.Key == key)?.Value;
            }

            public void Set(string key, object value)
            {
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
                if (value == null)
                {
                    this.isModifyData = this.isModifyData || this.Data.RemoveAll(q => q.Key == key) > 0;
                }
                else
                {
                    var m = this.Data.Find(q => q.Key == key);
                    if (m == null)
                    {
                        m = new SessionData() { Key = key };
                        this.Data.Add(m);
                    }

                    if(!value.Equals(m.Value))
                    {
                        m.Value = value;
                        this.isModifyData = true;
                    }
                }
            }

            public void Remove(string key)
            {
               this.isModifyData = this.isModifyData || this.Data.RemoveAll(q => q.Key == key) > 0;

            }

            public void Clear()
            {
                this.isModifyData = this.isModifyData || this.Data.Count > 0;
                this.Data.Clear();
            }

            public void Expire()
            {
                var datacache = IocUtils.Get<ISessionDataCache>();
                if (this.isModifyData)
                {
                    datacache.Set(this.Sid, this.Data);
                }
                else if (!string.IsNullOrEmpty(this.sid))
                {
                    datacache.Expire(this.sid);
                }

                var cache = IocUtils.Get<ISessionCache>();
                if (this.isModifyUser || this.user != null && this.user.IsModify)
                {
                    cache.Set(this.Sid, this.user);
                }
                else if(!string.IsNullOrEmpty(this.sid))
                {
                    cache.Expire(this.sid);
                }                
            }

            public void Logout()
            {
                if (!string.IsNullOrEmpty(this.sid))
                {
                    var cache = IocUtils.Get<ISessionCache>();
                    cache.Remove(this.sid);
                    var datacache = IocUtils.Get<ISessionDataCache>();
                    datacache.Remove(this.sid);
                }
                this.User = null;
                this.Data.Clear();
                this.isModifyData = false;
                this.RestSid();
            }
        }
    }
}
