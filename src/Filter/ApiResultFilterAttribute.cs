//-----------------------------------------------------------------------
// <copyright file="ApiResultFilterAttribute.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 11:55:56</date>
//-----------------------------------------------------------------------
using FastHttpApi.Entity.Other;
using FastHttpApi.Service.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System;

namespace FastHttpApi.Filter
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// OnActionExecuted
        /// </summary>
        /// <param name="context">ActionExecutedContext</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                ////记录错误
                var otherService = (IOtherService)context.HttpContext.RequestServices.GetService(typeof(IOtherService));

                otherService.SaveException(new ExceptionEntity
                {
                    Message = context.Exception.Message,
                    StackTrace = context.Exception.StackTrace,
                    TargetSite = context.Exception.TargetSite.DeclaringType.FullName,
                });

                var ex = context.Exception.InnerException != null ? context.Exception.InnerException : context.Exception;
                Log.Error(context.Exception, $"发生系统错误！！！！ {ex}");

                context.Result = new OkObjectResult(new
                {
                    Time = DateTime.UtcNow,
                    Type = "error",
                    Message = ex
                });

                context.ExceptionHandled = true;
            }
            else
            {
                switch (context.Result)
                {
                    case ObjectResult objectResult:
                        {
                            context.Result = new OkObjectResult(new
                            {
                                Time = DateTime.UtcNow,
                                Type = "success",
                                Result = objectResult.Value
                            });
                            break;
                        }

                    default:
                        break;
                }
            }
        }
    }
}