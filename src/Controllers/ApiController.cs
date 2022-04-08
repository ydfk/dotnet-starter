//-----------------------------------------------------------------------
// <copyright file="ApiController.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 11:18:41</date>
//-----------------------------------------------------------------------

using FastDotnet.Schema.App;
using FastDotnet.Schema.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDotnet.Controllers
{
    /// <summary>
    /// 提供服务接口
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("Api/[controller]")]
    public class ApiController : ControllerBase
    {
        public UserModel UserContext
        {
            get
            {
                if (HttpContext.Items.ContainsKey(AppConstant.UserContext))
                {
                    return (UserModel)HttpContext.Items[AppConstant.UserContext];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}