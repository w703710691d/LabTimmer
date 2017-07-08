using Newtonsoft.Json;
using System;
namespace Json_Helper
{
    internal class JsonHelper
    {
        public static string JsonSerializer<T>(T t)
        {
            return JsonConvert.SerializeObject(t);
        }
        public static T JsonDeserialize<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
