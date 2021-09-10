//-----------------------------------------------------------------------
// <copyright file="TestController.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 14:07:44</date>
//-----------------------------------------------------------------------

using FastHttpApi.Schema.User;
using FastHttpApi.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FastHttpApi.Controllers
{
    /// <summary>
    /// 测试
    /// </summary>
    public class TestController : ApiController
    {
        private readonly IUserService _userService;

        public TestController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns>当前用户</returns>
        [HttpGet]
        public string Get()
        {
            return UserContext.Id;
        }

        /// <summary>
        /// 测试新增用户
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>结果</returns>
        [HttpPost("User"), AllowAnonymous]
        public async Task<UserModel> AddUser([FromBody] UserModel user)
        {
            return await _userService.AddUser(new UserModel
            {
                UserName = user.UserName,
                Password = user.Password
            });
        }
    }
}