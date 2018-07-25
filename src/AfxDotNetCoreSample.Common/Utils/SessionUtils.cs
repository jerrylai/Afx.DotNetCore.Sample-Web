using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Afx.Cache;
using Afx.Utils;

namespace AfxDotNetCoreSample.Common
{
    public static class SessionUtils
    {
        [ThreadStatic]
        private static string _sid;

        public static void OnRequestSid(string sid)
        {
            _sid = !string.IsNullOrEmpty(sid) ? sid : Guid.NewGuid().ToString("n");
        }

        public static Action ResponseSidCall;

        public static string OnResponseSid(string sid)
        {
            var s = _sid;
            if (!string.IsNullOrEmpty(s))
            {
                ResponseSidCall?.Invoke();
            }
            _sid = null;

            return s;
        }

        public static string Sid
        {
            get
            {
                return _sid;
            }
        }

        public static void RestSid()
        {
            if (!string.IsNullOrEmpty(_sid))
            {
                _sid = Guid.NewGuid().ToString("n");
            }
        }
    }
}
