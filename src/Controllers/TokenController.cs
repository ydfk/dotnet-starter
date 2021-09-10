//-----------------------------------------------------------------------
// <copyright file="TokenController.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 16:09:43</date>
//-----------------------------------------------------------------------

using FastHttpApi.Service.Contract;
using FastHttpApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FastHttpApi.Controllers
{
    /// <summary>
    /// token相关
    /// </summary>
    public class TokenController : ApiController
    {
        private readonly IUserService _userService;

        public TokenController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>token</returns>
        [HttpGet, AllowAnonymous]
        public async Task<TokenRecord> GetToken(string userName, string password)
        {
            var user = await _userService.GetUserByUserName(userName);

            if (user != null && user.Password == SecurityUtil.Md5Password(userName, password))
            {
                return new TokenRecord(JwtUtil.GenerateToken(user.Id), DateTime.UtcNow.AddMinutes(-3).AddSeconds(AppSettings.JwtExpiration));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <returns>token</returns>
        [HttpGet("Refresh")]
        public TokenRecord RefreshToken()
        {
            return new TokenRecord(JwtUtil.GenerateToken(UserContext.Id), DateTime.UtcNow.AddMinutes(-3).AddSeconds(AppSettings.JwtExpiration));
        }
    }

    public sealed record TokenRecord(string Token, DateTime Expiration);
}