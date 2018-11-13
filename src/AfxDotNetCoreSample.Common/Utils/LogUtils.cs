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
        private static string _configFile;
        public static string ConfigFile
        {
            get
            {
                if (string.IsNullOrEmpty(_configFile))
                {
                    _configFile = PathUtils.GetFileFullPath("log4net.config");
                }

                return _configFile;
            }
        }

        private static ILog _default = null;
        private static ILoggerRepository defaultPepository;
        public static ILog GetLog(string name = null)
        {
            if (defaultPepository == null)
            {
                defaultPepository = LogManager.CreateRepository("DefaultRepository");
                if (File.Exists(ConfigFile))
                {
                    XmlConfigurator.ConfigureAndWatch(defaultPepository, new FileInfo(ConfigFile));
                }
            }
            
            if (string.IsNullOrEmpty(name))
            {
                name = "Default";
                if(_default == null) _default = LogManager.GetLogger(defaultPepository.Name, name);

                return _default;
            }

            return LogManager.GetLogger(defaultPepository.Name, name);
        }



        /// <summary>
        /// 默认 ILog
        /// </summary>
        public static ILog Default => GetLog();

        /// <summary>
        /// debug 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Debug(string msg, string logName = null)
        {
            ILog log = GetLog(logName);
            log.Debug(msg);
        }

        /// <summary>
        /// debug 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="ex">异常</param>
        public static void Debug(string msg, Exception ex, string logName = null)
        {
            ILog log = GetLog(logName);
            if (ex != null)
            {
                StringBuilder s = new StringBuilder();
                s.Append($"{msg}, ExceptionType: {ex.GetType().Name}, Message: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    s.Append(Environment.NewLine);
                    s.Append("---------------------InnerException-----------------------");
                    s.Append(Environment.NewLine);
                    s.Append($"ExceptionType: {ex.GetType().Name}, Message: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                }
                log.Debug(s.ToString());
            }
            else
            {
                log.Debug(msg);
            }
        }

        /// <summary>
        /// info 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Info(string msg, string logName = null)
        {
            ILog log = GetLog(logName);
            log.Info(msg);
        }

        /// <summary>
        /// info 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="ex">异常</param>
        public static void Info(string msg, Exception ex, string logName = null)
        {
            ILog log = GetLog(logName);
            if (ex != null)
            {
                StringBuilder s = new StringBuilder();
                s.Append($"{msg}, ExceptionType: {ex.GetType().Name}, Message: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    s.Append(Environment.NewLine);
                    s.Append("---------------------InnerException-----------------------");
                    s.Append(Environment.NewLine);
                    s.Append($"ExceptionType: {ex.GetType().Name}, Message: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                }
                log.Info(s.ToString());
            }
            else
            {
                log.Info(msg);
            }
        }

        /// <summary>
        /// warn 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Warn(string msg, string logName = null)
        {
            ILog log = GetLog(logName);
            log.Warn(msg);
        }

        /// <summary>
        /// warn 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="ex">异常</param>
        public static void Warn(string msg, Exception ex, string logName = null)
        {
            ILog log = GetLog(logName);
            if (ex != null)
            {
                StringBuilder s = new StringBuilder();
                s.Append($"{msg}, ExceptionType: {ex.GetType().Name}, Message: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    s.Append(Environment.NewLine);
                    s.Append("---------------------InnerException-----------------------");
                    s.Append(Environment.NewLine);
                    s.Append($"ExceptionType: {ex.GetType().Name}, Message: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                }
                log.Warn(s.ToString());
            }
            else
            {
                log.Warn(msg);
            }
        }

        /// <summary>
        /// error 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Error(string msg, string logName = null)
        {
            ILog log = GetLog(logName);
            log.Error(msg);
        }

        /// <summary>
        /// error 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="ex">异常</param>
        public static void Error(string msg, Exception ex, string logName = null)
        {
            ILog log = GetLog(logName);
            if (ex != null)
            {
                StringBuilder s = new StringBuilder();
                s.Append($"{msg}, ExceptionType: {ex.GetType().Name}, Message: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    s.Append(Environment.NewLine);
                    s.Append("---------------------InnerException-----------------------");
                    s.Append(Environment.NewLine);
                    s.Append($"ExceptionType: {ex.GetType().Name}, Message: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                }
                log.Error(s.ToString());
            }
            else
            {
                log.Error(msg);
            }
        }

        /// <summary>
        /// fatal 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Fatal(string msg, string logName = null)
        {
            ILog log = GetLog(logName);
            log.Fatal(msg);
        }

        /// <summary>
        /// fatal 级别日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="ex">异常</param>
        public static void Fatal(string msg, Exception ex, string logName = null)
        {
            ILog log = GetLog(logName);
            if (ex != null)
            {
                StringBuilder s = new StringBuilder();
                s.Append($"{msg}, ExceptionType: {ex.GetType().Name}, Message: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    s.Append(Environment.NewLine);
                    s.Append("---------------------InnerException-----------------------");
                    s.Append(Environment.NewLine);
                    s.Append($"ExceptionType: {ex.GetType().Name}, Message: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                }
                log.Fatal(s.ToString());
            }
            else
            {
                log.Fatal(msg);
            }
        }

        #region log


        public static string GetLogDir(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            string logDir = null;
            string configpath = ConfigFile;
            if (!string.IsNullOrEmpty(configpath) && File.Exists(configpath))
            {
                using (var fs = File.Open(configpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(fs);
                    var nodes = doc.SelectNodes("configuration/log4net/logger");
                    string appender = null;
                    foreach (XmlNode node in nodes)
                    {
                        if (node is XmlElement)
                        {
                            XmlElement ch = node as XmlElement;
                            if (ch.GetAttribute("name") == name)
                            {
                                XmlElement lch = null;
                                foreach (XmlNode lchn in ch)
                                {
                                    if (lchn is XmlElement && lchn.Name == "appender-ref")
                                    {
                                        lch = lchn as XmlElement;
                                        break;
                                    }
                                }
                                if (lch != null) appender = lch.GetAttribute("ref");
                                if (!string.IsNullOrEmpty(appender)) break;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(appender))
                    {
                        nodes = doc.SelectNodes("configuration/log4net/appender");
                        foreach (XmlNode node in nodes)
                        {
                            if (node is XmlElement)
                            {
                                XmlElement ch = node as XmlElement;
                                if (ch.GetAttribute("name") == appender)
                                {
                                    XmlElement lch = null;
                                    foreach (XmlNode lchn in ch)
                                    {
                                        if (lchn is XmlElement && lchn.Name == "file")
                                        {
                                            lch = lchn as XmlElement;
                                            break;
                                        }
                                    }
                                    if (lch != null) logDir = lch.GetAttribute("value");
                                    if (!string.IsNullOrEmpty(logDir)) break;
                                }
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(logDir))
            {
                logDir = PathUtils.GetDirectoryFullPath(logDir);
            }

            return logDir;
        }

        const string LEVEL = "ALL|DEBUG|INFO|WARN|ERROR|FATAL|OFF";
        public static bool SetLevel(string name, string level)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(level)) throw new ArgumentNullException(nameof(level));
            level = level.ToUpper();
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
                        var nodes = doc.SelectNodes("configuration/log4net/logger");//"configuration/log4net/root/level");
                        foreach (XmlNode node in nodes)
                        {
                            if (node is XmlElement)
                            {
                                XmlElement ch = node as XmlElement;
                                if (ch.GetAttribute("name") == name)
                                {
                                    XmlElement lch = null;
                                    foreach (XmlNode lchn in ch)
                                    {
                                        if (lchn is XmlElement && lchn.Name == "level")
                                        {
                                            lch = lchn as XmlElement;
                                            break;
                                        }
                                    }
                                    if (lch!= null && lch.GetAttribute("value")?.ToUpper() != level)
                                    {
                                        lch.SetAttribute("value", level);
                                        isok = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (isok)
                        {
                            fs.Seek(0, SeekOrigin.Begin);
                            fs.SetLength(0);
                            doc.Save(fs);
                            fs.Flush();
                        }
                    }
                }
            }

            return isok;
        }

        public static string GetLevel(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            string level = "";
            string configpath = ConfigFile;
            if (!string.IsNullOrEmpty(configpath) && File.Exists(configpath))
            {
                using (var fs = File.Open(configpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(fs);
                    var nodes = doc.SelectNodes("configuration/log4net/logger");//"configuration/log4net/root/level");
                    foreach (XmlNode node in nodes)
                    {
                        if (node is XmlElement)
                        {
                            XmlElement ch = node as XmlElement;
                            if (ch.GetAttribute("name") == name)
                            {
                                XmlElement lch = null;
                                foreach (XmlNode lchn in ch)
                                {
                                    if(lchn is XmlElement && lchn.Name == "level")
                                    {
                                        lch = lchn as XmlElement;
                                        break;
                                    }
                                }
                                if (lch != null) level = lch.GetAttribute("value");
                                if (!string.IsNullOrEmpty(level)) break;
                            }
                        }
                    }
                }
            }

            return level;
        }

        #endregion

    }
}
