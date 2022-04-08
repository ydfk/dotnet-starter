//-----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 13:41:38</date>
//-----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using System;

namespace FastDotnet
{
    public static class AppSettings
    {
        static AppSettings()
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env != null)
            {
                config.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);
            }

            Configuration = config
                .Build();
        }

        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// 跨域允许的源
        /// </summary>
        public static string CorsOrigins => GetStr("*", "CorsOrigins");

        #region jwt

        /// <summary>
        /// Jwt令牌过期时间(秒)
        /// </summary>
        public static int JwtExpiration => GetInt(1800, "Jwt", "Expiration");

        /// <summary>
        /// Jwt Secret
        /// </summary>
        public static string JwtSecret => GetStr("050195eaba5f487d825d7d2d783dd8d2", "Jwt", "Secret");

        /// <summary>
        /// Jwt Issuer
        /// </summary>
        public static string JwtIssuer => GetStr("FastDotnet", "Jwt", "Issuer");

        #endregion jwt

        #region Mongo

        public static string MongoConnection => GetStr("mongodb://127.0.0.1:27017", "Mongo", "Connection");

        public static string MongoDatabase => GetStr("fastHttpApi", "Mongo", "Database");

        public static bool ShowMongoLog => GetBool("Mongo", "ShowLog");

        #endregion Mongo

        #region Redis

        public static string RedisHost => GetStr("127.0.0.1", "Host", "Redis");
        public static int RedisPort => GetInt(6379, "Host", "Redis");
        public static string RedisPassword => GetStr(string.Empty, "Password", "Redis");

        #endregion Redis

        #region 公共

        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections">路径名称</param>
        /// <returns>配置内容(不存在返回空)</returns>
        private static string Get(params string[] sections)
        {
            return Configuration[string.Join(":", sections)];
        }

        private static int GetInt(int defaultVal, params string[] sections)
        {
            int.TryParse(Get(sections), out int val);
            return val == 0 ? defaultVal : val;
        }

        private static bool GetBool(params string[] sections)
        {
            bool.TryParse(Get(sections), out bool val);
            return val;
        }

        private static string GetStr(string defaultVal, params string[] sections)
        {
            var val = Get(sections);
            return string.IsNullOrWhiteSpace(val) ? defaultVal : val;
        }

        private static Guid GetGuid(string defaultVal, params string[] sections)
        {
            Guid.TryParse(GetStr(defaultVal, sections), out Guid result);
            return result == default ? new Guid(defaultVal) : result;
        }

        #endregion 公共
    }
}