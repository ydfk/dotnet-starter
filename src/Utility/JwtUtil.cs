//-----------------------------------------------------------------------
// <copyright file="JwtUtil.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 15:40:37</date>
//-----------------------------------------------------------------------
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace FastHttpApi.Utility
{
    public static class JwtUtil
    {
        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="id">用户认证信息</param>
        /// <returns>JWT加密字符</returns>
        public static string GenerateToken(string id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = AppSettings.JwtIssuer,
                Subject = new ClaimsIdentity(new[] { new Claim("id", id) }),
                Expires = DateTime.UtcNow.AddSeconds(AppSettings.JwtExpiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.JwtSecret)), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jwtStr">jwt字符串</param>
        /// <returns>用户唯一身份Id</returns>
        public static string SerializeJwt(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(jwtStr);
            return jwtToken.Claims.First(x => x.Type == "id").Value;
        }

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>验证是否通过</returns>
        public static bool VerifyToken(string token)
        {
            var secret = AppSettings.JwtSecret;
            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidIssuer = AppSettings.JwtIssuer,
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (SecurityTokenException)
            {
                return false;
            }
            catch
            {
                throw;
            }

            return validatedToken != null;
        }
    }
}