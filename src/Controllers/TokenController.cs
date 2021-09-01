//-----------------------------------------------------------------------
// <copyright file="TokenController.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 16:09:43</date>
//-----------------------------------------------------------------------

using FastHttpApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastHttpApi.Controllers
{
    public class TokenController : ApiController
    {
        [HttpGet, AllowAnonymous]
        public string GetToken(string userName, string password)
        {
            // TODO: 验证用户
            var userId = $"{userName}{password}";
            return JwtUtil.GenerateToken(userId);
        }
    }
}