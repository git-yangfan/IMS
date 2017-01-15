using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace IMS.Json
{
    /// <summary>
    /// json扩展
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// 将指定对象转换为Json字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="camelCase">是否使用骆驼命名法，默认false</param>
        /// <param name="indented">是否生成良好的显示格式，保留空格和换行符，默认false</param>
        /// <param name="timeFormat">时间格式，默认yyyy-MM-dd HH:mm:ss</param>
        /// <returns></returns>
        public static string ToJsonString(this object obj, bool camelCase = false, bool indented = false, string timeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            var options = new JsonSerializerSettings();
            if (camelCase)
            {
                options.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            if (indented)
            {
                options.Formatting = Formatting.Indented;
            }
            options.DateFormatString = timeFormat;
            return JsonConvert.SerializeObject(obj, options);
        }
    }
}
