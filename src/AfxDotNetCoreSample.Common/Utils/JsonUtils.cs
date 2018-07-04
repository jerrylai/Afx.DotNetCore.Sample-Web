using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace AfxDotNetCoreSample.Common
{
    public static class JsonUtils
    {
        public static string Serialize<T>(T value)
        {
            if (value == null) return null;

            return JsonConvert.SerializeObject(value);
        }

        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return default(T);

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
