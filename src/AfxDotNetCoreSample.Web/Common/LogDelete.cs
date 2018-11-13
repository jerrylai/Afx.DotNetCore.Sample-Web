using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AfxDotNetCoreSample.Common;

namespace AfxDotNetCoreSample.Web
{
    public static class LogDelete
    {
        private static bool isStart = false;

        public static void Start()
        {
            if (isStart) return;
            isStart = true;
            System.Threading.ThreadPool.QueueUserWorkItem(Exec);
        }

        private readonly static List<string> LOG_TYPE_LIST = new List<string>() { "Default", "Web", "SQL" };
        private static void Exec(object obj)
        {
            var day = ConfigUtils.LogSaveDay;
            while(true)
            {
                System.Threading.Thread.Sleep(5 * 60 * 1000);
                try
                {
                    DateTime exp = DateTime.Now.AddDays(-day);
                    foreach(var name in LOG_TYPE_LIST)
                    {
                        var path = LogUtils.GetLogDir(name);
                        if(System.IO.Directory.Exists(path))
                        {
                            var dir = new System.IO.DirectoryInfo(path);
                            var files = dir.EnumerateFiles().Where(q=>q.CreationTime < exp);
                            foreach(var f in files)
                            {
                                try { f.Delete(); }
                                catch { }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    LogUtils.Error("【删除日志】", ex);
                }
            }
        }
    }
}
