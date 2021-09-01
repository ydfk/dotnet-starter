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
using FastHttpApi.Entity.User;
using FastHttpApi.Repository;
using FastHttpApi.Schema.User;
using FastHttpApi.Service.Contract;
using FastHttpApi.Utility;

namespace FastHttpApi.Service.Implement
{
    public class UserService : IUserService
    {
        private readonly IRepository<UserEntity> _userRepository;

        public UserService(IRepository<UserEntity> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserModel> GetUserById(string id)
        {
            return await _userRepository.Get<UserModel>(x => x.Id == id);
        }

        public async Task<UserModel> GetUserByUserName(string userName)
        {
            return await _userRepository.Get<UserModel>(x => x.UserName == userName);
        }

        public async Task<UserModel> AddUser(UserModel user)
        {
            user.Password = SecurityUtil.Md5Password(user.UserName, user.Password);
            var existUser = await _userRepository.Get<UserModel>(x => x.UserName == user.UserName);

            if (existUser != null)
            {
                existUser.Password = user.Password;
                existUser.Name = user.Name;
                return await _userRepository.Update<UserModel>(existUser);
            }
            else
            {
                return await _userRepository.Save<UserModel>(user);
            }
        }
    }
}