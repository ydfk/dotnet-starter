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
                        ////���Կ��ֶ�
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                        ////����Ϊ������ֶ�
                        options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                        ////����ĸСд
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        ////ö������ַ���
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        ////��������
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
        ////������
        ValidIssuer = AppSettings.JwtIssuer,
        ValidateAudience = false,
        ////�Ƿ���֤��ʱ  ������exp��nbfʱ��Ч ͬʱ����ClockSkew
        ValidateLifetime = true,
        ////ע�����ǻ������ʱ�䣬�ܵ���Чʱ��������ʱ�����jwt�Ĺ���ʱ�䣬��������ã�Ĭ����5����
        ClockSkew = TimeSpan.FromMinutes(30),
        RequireExpirationTime = true,
    };
    option.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            // ������ڣ����<�Ƿ����>��ӵ�������ͷ��Ϣ��
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
        Title = "dotnet���ٿ���",
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
            Description = "JWT��Ȩֱ�����¿�������{token}\"",
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

#region ����cors

builder.Services.AddCors(c =>
{
    ////һ��������ַ���
    c.AddPolicy(
"LimitRequests",
policy =>
{
    var corsOrigins = AppSettings.CorsOrigins;
    ////��������
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

#endregion ����cors

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
    c.DocumentTitle = "���ٿ���";
});

////��֤
app.UseAuthentication();
////��Ȩ
app.UseAuthorization();

////�������
app.UseCors("LimitRequests");

////���ش�����
app.UseStatusCodePages();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();

#endregion App