//-----------------------------------------------------------------------
// <copyright file="JwtMiddleware.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 15:32:25</date>
//-----------------------------------------------------------------------

using FastHttpApi.Repository;
using FastHttpApi.Schema.App;
using FastHttpApi.Schema.User;
using FastHttpApi.Utility;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace FastHttpApi.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, DbContext dbContext)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await attachUserToContext(context, dbContext, token);
            }

            await _next(context);
        }

        private async Task attachUserToContext(HttpContext context, DbContext dbContext, string token)
        {
            if (JwtUtil.VerifyToken(token))
            {
                var userId = JwtUtil.SerializeJwt(token);
                // TODO: 数据库查询用户信息
                context.Items[AppConstant.UserContext] = new UserModel
                {
                    Id = userId
                };
            }
        }
    }
}