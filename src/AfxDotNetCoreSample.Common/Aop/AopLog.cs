using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Afx.DynamicProxy;

namespace AfxDotNetCoreSample.Common
{
    public class AopLog : IAop
    {
        public virtual void OnException(AopContext context, Exception ex)
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendFormat("【AopLog.OnException】Class: {0}, Method: {1};", context.TargetType.FullName, context.Method.Name);
            var param = context.Method.GetParameters();
            object[] args = context.Arguments ?? new object[0];
            for (int i = 0; i < param.Length; i++)
            {
                var p = param[i];
                if (!p.IsOut && !typeof(Delegate).IsAssignableFrom(p.ParameterType))
                {
                    try
                    {
                        msg.AppendFormat("\r\n{0}: {1}", p.Name, args.Length > i ? (args[i] == null ? "null" : JsonUtils.Serialize(args[i])) : "");
                    }
                    catch { }
                }
            }
            msg.Append("异常：");

            if (ex is ApiException)
                LogUtils.Info(msg.ToString(), ex);
            else
                LogUtils.Error(msg.ToString(), ex);
        }

        static readonly DateTime STOP_TIME = new DateTime(2019, 1, 1);
        public virtual void OnExecuting(AopContext context)
        {
            var now = DateTime.Now;
            context.UserState = now;
        }

        public virtual void OnResult(AopContext context, object returnValue)
        {
            var starttime = (DateTime)context.UserState;
            var time = DateTime.Now - starttime;
            LogUtils.Debug($"【AOP】Class: {context.TargetType.FullName}, Method: {context.Method.Name}, TotalMilliseconds: {time.TotalMilliseconds}", WebLogger.LOG_NAME);
        }
    }
}
