//-----------------------------------------------------------------------
// <copyright file="RedisUtil.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 14:01:04</date>
//-----------------------------------------------------------------------

using StackExchange.Redis.Extensions.Core.Configuration;

namespace FastHttpApi.Utility
{
    public class RedisUtil
    {
        public static RedisConfiguration GetRedisConfiguration()
        {
            var redisConfiguration = new RedisConfiguration()
            {
                AbortOnConnectFail = true,
                KeyPrefix = "data_lake_",
                Hosts = new[]
                {
                    new RedisHost()
                    {
                        Host = AppSettings.RedisHost,
                        Port = AppSettings.RedisPort
                    }
                },
                Database = 0,
                Ssl = false,
                ConnectTimeout = 10000,
                AllowAdmin = true,
            };

            if (!string.IsNullOrEmpty(AppSettings.RedisPassword))
            {
                redisConfiguration.Password = AppSettings.RedisPassword;
            }

            return redisConfiguration;
        }
    }
}