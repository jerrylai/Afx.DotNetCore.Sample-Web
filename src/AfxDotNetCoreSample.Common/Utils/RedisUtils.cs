using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace AfxDotNetCoreSample.Common
{
   public static  class RedisUtils
    {
        private static Lazy<IConnectionMultiplexer> _defaultConnectionMultiplexer = new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(ConfigUtils.RedisConfig), true);

        public static IConnectionMultiplexer Default
        {
            get { return _defaultConnectionMultiplexer.Value; }
        }

        public static IDatabase GetDatabase(int db = -1)
        {
           
           return  Default.GetDatabase(db);
        }
    }
}
