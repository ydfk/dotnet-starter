//-----------------------------------------------------------------------
// <copyright file="JsonUtil.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 13:59:10</date>
//-----------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace FastHttpApi.Utility
{
    public static class JsonUtil
    {
        /// <summary>
        /// 获得默认设置
        /// </summary>
        /// <returns>JsonSerializerSettings</returns>
        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                ////忽略空字段
                NullValueHandling = NullValueHandling.Ignore,
                ////忽略为定义的字段
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ////首字母小写
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ////枚举输出字符串
                Converters = new List<JsonConverter>() { new StringEnumConverter() },
                ////中文乱码
                StringEscapeHandling = StringEscapeHandling.Default,
            };
        }

        /// <summary>
        /// 将对象序列化为Json
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>Json</returns>
        public static string JsonFromObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, GetJsonSerializerSettings());
        }

        public static T ObjectFromJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(json, JsonUtil.GetJsonSerializerSettings());
        }

        public static object ObjectFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonConvert.DeserializeObject(json, JsonUtil.GetJsonSerializerSettings());
        }
    }
}