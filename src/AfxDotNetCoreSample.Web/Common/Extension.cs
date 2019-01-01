using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.IService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AfxDotNetCoreSample.Web
{
    public static class Extension
    {
        private const string USER_SESSION_KEY = "__AFX_USER_SESSION";
        private const string SET_USER_SESSION_KEY = "__AFX_SET_USER_SESSION";

        private const string IFRAME_AJAX = "_iframe_ajax";

        public static bool IsAjax(this HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            bool result = false;
            if (string.Equals(request.Headers["x-requested-with"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
            {
                result = true;
            }
            else
            {
                result = IsIFrameAjax(request);
            }

            return result;
        }

        public static bool IsIFrameAjax(this HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            bool result = false;
            if(string.Equals(request.Method, "get", StringComparison.OrdinalIgnoreCase))
            {
                result = string.Equals(request.Query[IFRAME_AJAX], "true");
            }
            else if (string.Equals(request.Method, "post", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrEmpty(request.ContentType))
            {
                try { result = string.Equals(request.Form[IFRAME_AJAX], "true"); }
                catch { }
            }

            return result;
        }

        public static bool IsPost(this HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            bool result = string.Equals("post", request.Method, StringComparison.OrdinalIgnoreCase);

            return result;
        }

        public static bool IsGet(this HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            bool result = string.Equals("get", request.Method, StringComparison.OrdinalIgnoreCase);

            return result;
        }

        public static string RefreshSid(this HttpContext httpContext, string sid, TimeSpan? sidExpire, double minRefExpire)
        {
            var s = sid;
            string setState = httpContext.Items[SET_USER_SESSION_KEY] as string;
            if (setState == "0")
            {
                s = Guid.NewGuid().ToString("n");
            }
            else if (sidExpire.HasValue)
            {
                var arr = s.Split('-');
                long ticks = 0;
                if (arr.Length == 2)
                {
                    long.TryParse(arr[1], out ticks);
                }
                var now = DateTime.Now;
                if (setState == "1" || (ticks > 0 && (new DateTime(ticks) - now).TotalMinutes < minRefExpire))
                {
                    var userinfo = httpContext.GetUserSession();
                    if (userinfo != null)
                    {
                        using (var sessionService = IocUtils.Get<IService.IUserSessionService>())
                        {
                            sessionService.Expire(arr[0]);
                            ticks = now.Add(sidExpire.Value).Ticks;
                            LogUtils.Debug($"【刷新session】sid: {arr[0]}, Name: {userinfo.Name}, Account: {userinfo.Account}, LoginTime: {userinfo.LoginTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                            s = $"{arr[0]}-{ticks}";
                        }
                    }
                }
            }

            httpContext.Items[SessionUtils.SidName] = s;

            return s;
        }

        public static string GetSid(this HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            var sid = httpContext.Items[SessionUtils.SidName] as string;
            if (!string.IsNullOrEmpty(sid))
            {
                var arr = sid.Split('-');
                sid = arr[0];
            }
            return sid;
        }

        public static UserSessionDto GetUserSession(this HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            UserSessionDto m = httpContext.Items[USER_SESSION_KEY] as UserSessionDto;
            if (m == null)
            {
                var sid = GetSid(httpContext);
                if (!string.IsNullOrEmpty(sid))
                {
                    using (var userSessionService = IocUtils.Get<IUserSessionService>())
                    {
                        m = userSessionService.Get(sid);
                        if (m != null) httpContext.Items[USER_SESSION_KEY] = m;
                    }
                }
            }

            return m;
        }

        public static void SetUserSession(this HttpContext httpContext, UserSessionDto m)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            var sid = GetSid(httpContext);
            if (!string.IsNullOrEmpty(sid))
            {
                using (var userSessionService = IocUtils.Get<IUserSessionService>())
                {
                    if (m != null) m.Sid = sid;
                    userSessionService.Set(sid, m);
                    httpContext.Items[USER_SESSION_KEY] = m;
                    httpContext.Items[SET_USER_SESSION_KEY] = m != null ? "1" : "0";
                }
            }
        }

        public static void ClearUserSession(this HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            var sid = GetSid(httpContext);
            if (!string.IsNullOrEmpty(sid))
            {
                using (var userSessionService = IocUtils.Get<IUserSessionService>())
                {
                    userSessionService.Logout(sid);
                    httpContext.Items[USER_SESSION_KEY] = null;
                    httpContext.Items[SET_USER_SESSION_KEY] = "0";
                }
            }
        }
    }
}
