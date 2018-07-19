using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace AfxDotNetCoreSample.Common
{
    public static class RedisUtils
    {
        private static Lazy<IConnectionMultiplexer> _default = new Lazy<IConnectionMultiplexer>(() =>
        {
            var con = ConnectionMultiplexer.Connect(ConfigUtils.RedisConfig);
            con.ConnectionFailed += OnConnectionFailed;
            con.ErrorMessage += OnErrorMessage;
            con.InternalError += OnInternalError;
            return con;
        }, true);

        public static IConnectionMultiplexer Default => _default.Value;


        private static void OnInternalError(object sender, InternalErrorEventArgs e)
        {
            LogUtils.Error($"【Redis.InternalError】ConnectionType:{e.ConnectionType}, EndPoint: {e.EndPoint}, Origin: {e.Origin}", e.Exception);
        }

        private static void OnErrorMessage(object sender, RedisErrorEventArgs e)
        {
            LogUtils.Error($"【Redis.InternalError】EndPoint: {e.EndPoint}, error: {e.Message}");
        }

        private static void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            LogUtils.Error($"【Redis.InternalError】ConnectionType:{e.ConnectionType}, EndPoint: {e.EndPoint}, FailureType: {e.FailureType}", e.Exception);
        }

        private static byte[] ToBytes<T>(T value)
        {
            byte[] buffer = null;
            if (value != null)
            {
                if (value is byte[])
                {
                    buffer = value as byte[];
                }
                else if (value is string)
                {
                    string json = value as string;
                    buffer = Encoding.UTF8.GetBytes(json);
                }
                else
                {
                    string json = JsonUtils.Serialize(value);
                    buffer = Encoding.UTF8.GetBytes(json);
                }
            }

            return buffer;
        }

        private static T FromBytes<T>(byte[] buffer)
        {
            T m = default(T);
            if (buffer != null)
            {
                if (typeof(T) == typeof(byte[]))
                {
                    m = (T)((object)buffer);
                }
                else if (buffer.Length > 0)
                {
                    string s = Encoding.UTF8.GetString(buffer);
                    if (typeof(T) == typeof(string))
                    {
                        m = (T)((object)s);
                    }
                    else if (!string.IsNullOrEmpty(s))
                    {
                        m = JsonUtils.Deserialize<T>(s);
                    }
                }
            }

            return m;
        }

        public static long Publish<T>(string channel, T value)
        {
            if (string.IsNullOrEmpty(channel)) throw new ArgumentNullException("channel");
            long count = 0;
            byte[] buffer = ToBytes(value);

            if (buffer != null && buffer.Length > 0)
            {
                var sub = Default.GetSubscriber();
                var prefix = ConfigUtils.CachePrefix;
                var s = $"{prefix}{channel}";
                count = sub.Publish(s, buffer);
            }

            return count;
        }

        public static void Subscribe<T>(string channel, Action<string, T> handler)
        {
            if (!string.IsNullOrEmpty(channel) && handler != null)
            {
                var sub = Default.GetSubscriber();
                sub.Subscribe(channel, (ch, v) =>
                {
                    T m = FromBytes<T>(v);
                    string s = ch.ToString();
                    var prefix = ConfigUtils.CachePrefix;
                    if (s.StartsWith(prefix)) s = s.Substring(prefix.Length);
                    try { handler(s, m); }
                    catch (Exception ex)
                    {
                        LogUtils.Error("【Subscribe】", ex);
                    }
                });
            }
        }
    }
}
