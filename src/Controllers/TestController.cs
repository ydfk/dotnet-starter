//-----------------------------------------------------------------------
// <copyright file="TestController.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 14:07:44</date>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using FastHttpApi.Schema.User;
using FastHttpApi.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastHttpApi.Controllers
{
    public class TestController : ApiController
    {
        private readonly IUserService _userService;

        public TestController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public string Get()
        {
            return UserContext.Id;
        }

        [HttpPost("user"), AllowAnonymous]
        public async Task<UserModel> AddUser(string userName, string password)
        {
            return await _userService.AddUser(new UserModel
            {
                UserName = userName,
                Password = password
            });
        }
    }
}