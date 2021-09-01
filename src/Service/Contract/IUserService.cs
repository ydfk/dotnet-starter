//-----------------------------------------------------------------------
// <copyright file="UserService.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 16:09:43</date>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastHttpApi.Schema.User;

namespace FastHttpApi.Service.Contract
{
    public interface IUserService
    {
        Task<UserModel> GetUserById(string id);

        Task<UserModel> AddUser(UserModel user);

        Task<UserModel> GetUserByUserName(string userName);
    }
}