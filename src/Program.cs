using Autofac;
using Autofac.Extensions.DependencyInjection;
using FastDotnet;
using FastDotnet.Filter;
using FastDotnet.Middleware;
using FastDotnet.Repository;
using FastDotnet.Service.Implement;
using FastDotnet.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) =>
{
    logger.ReadFrom.Configuration(context.Configuration);
});

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>((_, containerBuilder) =>
{
    containerBuilder.RegisterAssemblyTypes(typeof(OtherService).Assembly)
        .Where(t => t.Name.EndsWith("Service"))
        .AsImplementedInterfaces();
    containerBuilder.RegisterGeneric(typeof(Repository<>))
        .As(typeof(IRepository<>))
        .AsImplementedInterfaces();
});

#region services

builder.Services.AddControllers(o =>
                {
                    o.Filters.Add(typeof(ApiResultFilterAttribute));
                })
                .AddNewtonsoftJson(options =>
                    {
                        ////忽略空字段
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                        ////忽略为定义的字段
                        options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                        ////首字母小写
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        ////枚举输出字符串
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        ////中文乱码
                        options.SerializerSettings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
                    });

#region jwt

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.IncludeErrorDetails = true;
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.JwtSecret)),
        ValidateIssuer = true,
        ////发行人
        ValidIssuer = AppSettings.JwtIssuer,
        ValidateAudience = false,
        ////是否验证超时  当设置exp和nbf时有效 同时启用ClockSkew
        ValidateLifetime = true,
        ////注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟
        ClockSkew = TimeSpan.FromMinutes(30),
        RequireExpirationTime = true,
    };
    option.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            // 如果过期，则把<是否过期>添加到，返回头信息中
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }

            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken))
            {
                // Read the token out of the query string
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        },
    };
});

#endregion jwt

#region swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
        Title = "dotnet快速开发",
        Contact = new OpenApiContact()
        {
            Name = "ydfk",
            Email = "lyh6728326@gmail.com",
        }
    });

    c.AddSecurityDefinition(
        JwtBearerDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme()
        {
            Description = "JWT授权直接在下框中输入{token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
        });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
                    { new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference()
                        {
                            Id = JwtBearerDefaults.AuthenticationScheme,
                            Type = ReferenceType.SecurityScheme
                        }
                    }, Array.Empty<string>() }
    });

    c.CustomOperationIds(apiDesc =>
    {
        var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
        return controllerAction.ControllerName + "-" + controllerAction.ActionName;
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.AddSwaggerGenNewtonsoftSupport();

#endregion swagger

#region 跨域cors

builder.Services.AddCors(c =>
{
    ////一般采用这种方法
    c.AddPolicy(
"LimitRequests",
policy =>
{
    var corsOrigins = AppSettings.CorsOrigins;
    ////允许所有
    if (string.IsNullOrEmpty(corsOrigins) || corsOrigins == "*")
    {
        policy.SetIsOriginAllowed(_ => true)
         .AllowAnyHeader()
         .AllowAnyMethod().AllowCredentials();
    }
    else
    {
        var origins = new List<string>() { };

        if (corsOrigins.Contains(","))
        {
            origins = corsOrigins.Split(",").ToList();
        }
        else
        {
            origins.Add(corsOrigins);
        }

        policy.WithOrigins(origins.ToArray())
         .AllowAnyHeader()
         .AllowAnyMethod().AllowCredentials();
    }
});
});

#endregion 跨域cors

#region redis

builder.Services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(RedisUtil.GetRedisConfiguration());

#endregion redis

#endregion services

#region App

var app = builder.Build();

app.UseStaticFiles();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1");
    c.RoutePrefix = string.Empty;
    c.DocumentTitle = "快速开发";
});

////认证
app.UseAuthentication();
////授权
app.UseAuthorization();

////允许跨域
app.UseCors("LimitRequests");

////返回错误码
app.UseStatusCodePages();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();

#endregion App