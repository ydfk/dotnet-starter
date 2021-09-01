//-----------------------------------------------------------------------
// <copyright file="OtherService.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 13:33:41</date>
//-----------------------------------------------------------------------

using FastHttpApi.Entity.Other;
using FastHttpApi.Repository;
using FastHttpApi.Service.Contract;
using System.Threading.Tasks;

namespace FastHttpApi.Service.Implement
{
    public class OtherService : IOtherService
    {
        private readonly IRepository<ExceptionEntity> _exceptionRepository;

        public OtherService(IRepository<ExceptionEntity> exceptionRepository)
        {
            _exceptionRepository = exceptionRepository;
        }

        public async Task SaveException(ExceptionEntity exception)
        {
            exception.CreateAt = System.DateTime.Now;
            exception.UpdateAt = System.DateTime.Now;
            exception.DataStatus = true;
            await _exceptionRepository.Save(exception);
        }
    }
}