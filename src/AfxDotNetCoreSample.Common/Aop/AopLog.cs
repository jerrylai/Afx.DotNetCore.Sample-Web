using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Afx.Aop;
using AfxDotNetCoreSample.Dto;

namespace AfxDotNetCoreSample.Common
{
    public class AopLog : IAop
    {
        public void OnException(AopContext context, Exception ex)
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendFormat("【AopLog.OnException】Class: {0}, Method: {1};", context.TagetType.FullName, context.Method.Name);
            var param = context.Method.GetParameters();
            object[] args = context.Parameters ?? new object[0];
            for (int i = 0; i < param.Length; i++)
            {
                var p = param[i];
                if(!p.IsOut)
                    msg.AppendFormat("\r\n{0}: {1}", p.Name, args.Length > i ? (args[i] == null ? "null" : JsonUtils.Serialize(args[i])) : "");
            }
            msg.Append("异常：");

            if(ex is ApiException)
                LogUtils.Info(msg.ToString(), ex);
            else
                LogUtils.Error(msg.ToString(), ex);
        }

        public void OnExecuting(AopContext context)
        {
            //context.UserState = DateTime.Now;
        }

        public void OnResult(AopContext context, object returnValue)
        {
            //if (context.UserState is DateTime)
            //{
            //    var startTime = (DateTime)context.UserState;
            //    var time = DateTime.Now - startTime;
            //    LogUtils.Debug(string.Format("【AopLog.Stopwatch】TotalMilliseconds: {0:f0}, Class: {1}, Method: {2}",
            //        time.TotalMilliseconds, context.TagetType.FullName, context.Method.Name));
            //}
        }
    }
}
