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
        public static bool IsAjax(this HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            bool result = false;
            Microsoft.Extensions.Primitives.StringValues v;
            if (request.Headers.TryGetValue("x-requested-with", out v) && v.FirstOrDefault() == "XMLHttpRequest"
                || string.Equals(request.Method, "post", StringComparison.OrdinalIgnoreCase)
                && request.Form.TryGetValue("_iframe_ajax", out v) && v.FirstOrDefault() == "true")
            {
                result = true;
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

        public static string RefreshSid(this HttpContext httpContext, TimeSpan? sidExpire, double minRefExpire)
        {
            string sid = httpContext.Items[SessionUtils.SidName] as string;
            string setState = httpContext.Items[SET_USER_SESSION_KEY] as string;
            if (setState == "0")
            {
                sid = Guid.NewGuid().ToString("n");
            }
            else if (sidExpire.HasValue)
            {
                var arr = sid.Split('-');
                long ticks = 0;
                if (arr.Length == 2)
                {
                    long.TryParse(arr[1], out ticks);
                }
                var now = DateTime.Now;
                if (setState == "1" || (ticks > 0 && (new DateTime(ticks) - now).TotalMinutes < minRefExpire))
                {
                    var sessionService = IocUtils.Get<IService.IUserSessionService>();
                    sessionService.Expire(sid);
                    ticks = now.Add(sidExpire.Value).Ticks;
                    sid = $"{arr[0]}-{ticks}";
                }
            }

            httpContext.Items[SessionUtils.SidName] = sid;

            return sid;
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

        const string USER_SESSION_KEY = "__AFX_USER_SESSION";
        const string SET_USER_SESSION_KEY = "__AFX_SET_USER_SESSION";
        public static UserSessionDto GetUserSession(this HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            UserSessionDto m = httpContext.Items[USER_SESSION_KEY] as UserSessionDto;
            if (m == null)
            {
                var sid = GetSid(httpContext);
                if (!string.IsNullOrEmpty(sid))
                {
                    var userSessionService = IocUtils.Get<IUserSessionService>();
                    m = userSessionService.Get(sid);
                    if (m != null) httpContext.Items[USER_SESSION_KEY] = m;
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
                var userSessionService = IocUtils.Get<IUserSessionService>();
                if (m != null) m.Sid = sid;
                userSessionService.Set(sid, m);
                httpContext.Items[USER_SESSION_KEY] = m;
                httpContext.Items[SET_USER_SESSION_KEY] = m != null ? "1" : "0";
            }
        }

        public static void ClearUserSession(this HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            var sid = GetSid(httpContext);
            if (!string.IsNullOrEmpty(sid))
            {
                var userSessionService = IocUtils.Get<IUserSessionService>();
                userSessionService.Logout(sid);
                httpContext.Items[USER_SESSION_KEY] = null;
                httpContext.Items[SET_USER_SESSION_KEY] = "0";
            }
        }
    }
}
