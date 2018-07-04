using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Afx.Utils;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace AfxDotNetCoreSample.Common
{
    public static class LogUtils
    {
        /// <summary>
        /// log配置文件
        /// </summary>
        public static string ConfigFile { get; set; }

        private static Lazy<ILog> _default = new Lazy<ILog>(GetLog, true);

        private static ILoggerRepository defaultPepository;
        private static ILog GetLog()
        {
            if (string.IsNullOrEmpty(ConfigFile))
            {
                ConfigFile = PathUtils.GetFileFullPath("log4net.config");
            }
            if (defaultPepository == null)
            {
                defaultPepository = LogManager.CreateRepository("DefaultRepository");
            }
            if (File.Exists(ConfigFile))
            {
                XmlConfigurator.ConfigureAndWatch(defaultPepository, new FileInfo(ConfigFile));
            }

            return LogManager.GetLogger(defaultPepository.Name, "Default");
        }



        /// <summary>
        /// 默认 ILog
        /// </summary>
        public static ILog Default => _default.Value;

        /// <summary>
        /// debug 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Debug(string msg)
        {
            Default.Debug(msg);
        }

        /// <summary>
        /// debug 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="ex">异常</param>
        public static void Debug(string msg, Exception ex)
        {
            Default.Debug($"{msg}{ex?.Message}{ex?.StackTrace}");
        }

        /// <summary>
        /// info 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Info(string msg, bool isTriggerWriteEvent = true)
        {
            Default.Info(msg);
        }

        /// <summary>
        /// info 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="ex">异常</param>
        public static void Info(string msg, Exception ex, bool isTriggerWriteEvent = true)
        {
            Default.Info($"{msg}{ex?.Message}{ex?.StackTrace}");
        }

        /// <summary>
        /// warn 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Warn(string msg)
        {
            Default.Warn(msg);
        }

        /// <summary>
        /// warn 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="ex">异常</param>
        public static void Warn(string msg, Exception ex)
        {
            Default.Warn($"{msg}{ex?.Message}{ex?.StackTrace}");
        }

        /// <summary>
        /// error 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Error(string msg)
        {
            Default.Error(msg);
        }

        /// <summary>
        /// error 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="ex">异常</param>
        public static void Error(string msg, Exception ex)
        {
            Default.Error($"{msg}{ex?.Message}{ex?.StackTrace}");
        }

        /// <summary>
        /// fatal 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Fatal(string msg)
        {
            Default.Fatal(msg);
        }

        /// <summary>
        /// fatal 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="ex">异常</param>
        public static void Fatal(string msg, Exception ex)
        {
            Default.Fatal($"{msg}{ex?.Message}{ex?.StackTrace}");
        }

        #region log


        private static string logDir = null;
        public static string GetLogDir()
        {
            if (logDir == null)
            {
                string configpath = ConfigFile;
                if (!string.IsNullOrEmpty(configpath) && File.Exists(configpath))
                {
                    using (var fs = File.Open(configpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(fs);
                        var node = doc.SelectSingleNode("configuration/log4net/appender/file");
                        if (node != null)
                        {
                            XmlElement ch = node as XmlElement;
                            if (ch != null) logDir = ch.GetAttribute("value");
                        }
                    }
                }
            }

            if (logDir == null)
            {
                logDir = "log\\";
            }

            logDir = PathUtils.GetDirectoryFullPath(logDir);

            return logDir;
        }

        const string LEVEL = "ALL|DEBUG|INFO|WARN|ERROR|FATAL|OFF";
        public static bool SetLevel(string level)
        {
            level = level?.ToUpper();
            bool isok = false;
            if (!string.IsNullOrEmpty(level) && LEVEL.Split('|').Contains(level))
            {
                string configpath = ConfigFile;
                if (!string.IsNullOrEmpty(configpath) && File.Exists(configpath))
                {
                    using (var fs = File.Open(configpath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(fs);
                        var node = doc.SelectSingleNode("configuration/log4net/root/level");
                        if (node != null)
                        {
                            XmlElement ch = node as XmlElement;
                            if (ch != null)
                            {
                                if (ch.GetAttribute("value")?.ToUpper() != level)
                                {
                                    ch.SetAttribute("value", level);
                                    fs.Seek(0, SeekOrigin.Begin);
                                    fs.SetLength(0);
                                    doc.Save(fs);
                                    fs.Flush();
                                }
                                isok = true;
                            }
                        }
                    }
                }
            }

            return isok;
        }

        public static string GetLevel()
        {
            string level = "";
            string configpath = ConfigFile;
            if (!string.IsNullOrEmpty(configpath) && File.Exists(configpath))
            {
                using (var fs = File.Open(configpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(fs);
                    var node = doc.SelectSingleNode("configuration/log4net/root/level");
                    if (node != null)
                    {
                        XmlElement ch = node as XmlElement;
                        if (ch != null)
                        {
                            level = ch.GetAttribute("value");
                        }
                    }
                }
            }

            return level;
        }

        public static void DeleteLogs(string date = null)
        {
            try
            {
                string dir = GetLogDir();
                if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                {
                    DateTime deltime = DateTime.Now.AddHours(-1);
                    if (string.IsNullOrEmpty(date) && !DateTime.TryParse(date, out deltime))
                    {
                        int day = ConfigUtils.LogSaveDay;
                        if (day > 0) deltime = DateTime.Now.Date.AddDays(-day);
                        else deltime = DateTime.Now.AddHours(-1);
                    }

                    System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(dir);
                    var files = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Where(q => q.CreationTime <= deltime);
                    foreach (var f in files)
                    {
                        try { f.Delete(); }
                        catch { }
                    }
                }
            }
            catch { }
        }

        #endregion

    }
}
