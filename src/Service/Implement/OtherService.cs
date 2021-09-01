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
        private readonly DbContext _dbContext;

        public OtherService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveException(ExceptionEntity exception)
        {
            exception.CreateAt = System.DateTime.Now;
            exception.UpdateAt = System.DateTime.Now;
            exception.DataStatus = true;
            await _dbContext.GetCollection<ExceptionEntity>().InsertOneAsync(exception);
        }
    }
}