//-----------------------------------------------------------------------
// <copyright file="TokenController.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 16:09:43</date>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using FastHttpApi.Service.Contract;
using FastHttpApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastHttpApi.Controllers
{
    public class TokenController : ApiController
    {
        private readonly IUserService _userService;

        public TokenController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet, AllowAnonymous]
        public async Task<string> GetToken(string userName, string password)
        {
            var user = await _userService.GetUserByUserName(userName);

            if (user != null && user.Password == SecurityUtil.Md5Password(userName, password))
            {
                return JwtUtil.GenerateToken(user.Id);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}