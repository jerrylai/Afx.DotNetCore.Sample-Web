using System;
using System.Collections.Generic;
using System.Text;

using Afx.Map;

namespace AfxDotNetCoreSample.Common
{
    public static class MapUtils
    {
        private static Lazy<MapFactory> _mapFactory = new Lazy<MapFactory>(() => IocUtils.Get<MapFactory>(), false);

        public static MapFactory Default => _mapFactory.Value;

        public static object To(object obj, Type resultType)
        {
            if (obj == null) return null;
            if (resultType == null) throw new ArgumentNullException(nameof(resultType));
            if(resultType == typeof(object)) throw new ArgumentException(nameof(resultType) + " 类型错误！");
            if (!(!resultType.IsAbstract && resultType.IsClass)) throw new ArgumentException(nameof(resultType) + " 类型错误！");

            return Default.To(obj, resultType);
        }

        public static T To<T>(object obj)
        {
            if (obj == null) return default(T);
            var resultType = typeof(T);
            if (resultType == typeof(object)) throw new ArgumentException("T 类型错误！");
            if (!(!resultType.IsAbstract && resultType.IsClass)) throw new ArgumentException("T 类型错误！");

            return Default.To<T>(obj);
        }
    }
}
