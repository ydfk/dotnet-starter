//-----------------------------------------------------------------------
// <copyright file="TestController.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 14:07:44</date>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace FastHttpApi.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        public string Get()
        {
            return UserContext.Id;
        }
    }
}