//-----------------------------------------------------------------------
// <copyright file="DbContext.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 13:53:58</date>
//-----------------------------------------------------------------------
using FastHttpApi.Entity.Base;
using FastHttpApi.Schema.Base;
using Mapster;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using MongoDB.Driver.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FastHttpApi.Repository
{
    public class DbContext
    {
        public IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            string tableName = BsonClassMap.LookupClassMap(typeof(TEntity)).Discriminator;
            var client = GetClient(AppSettings.MongoConnection, AppSettings.ShowMongoLog);
            var database = client.GetDatabase(AppSettings.MongoDatabase);
            return database.GetCollection<TEntity>(tableName);
        }

        public async Task<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity
        {
            return await GetCollection<TEntity>().Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IList<TEntity>> List<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity
        {
            return await GetCollection<TEntity>().Find(filter).ToListAsync();
        }

        public async Task<IList<TEntity>> List<TEntity>() where TEntity : BaseEntity
        {
            return await GetCollection<TEntity>().Find(Builders<TEntity>.Filter.Empty).ToListAsync();
        }

        public IMongoQueryable<TEntity> QueryData<TEntity>()
        {
            return GetCollection<TEntity>().AsQueryable();
        }

        public async Task<IList<TEntity>> ListByIds<TEntity>(IList<string> ids) where TEntity : BaseEntity
        {
            var builderFilter = Builders<TEntity>.Filter;
            var query = builderFilter.In(x => x.Id, ids);

            return (await GetCollection<TEntity>().FindAsync(query)).ToList();
        }

        public PageResultModel<TModel> PageData<TEntity, TModel>(Expression<Func<TEntity, bool>> filter, PageQueryModel pageQery)
        {
            var pageResult = new PageResultModel<TModel>
            {
                TotalCount = QueryData<TEntity>().Count(),
                PageIndex = pageQery.PageIndex,
                PageSize = pageQery.PageSize,
            };

            List<TEntity> list;
            if (filter != null)
            {
                list = QueryData<TEntity>().Where(filter).Skip(pageQery.PageSize * (pageQery.PageIndex - 1)).Take(pageQery.PageSize).ToList();
            }
            else
            {
                list = QueryData<TEntity>().Skip(pageQery.PageSize * (pageQery.PageIndex - 1)).Take(pageQery.PageSize).ToList();
            }

            pageResult.Data = list.Adapt<IList<TModel>>();

            return pageResult;
        }

        public async Task<long> Count<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity
        {
            return await GetCollection<TEntity>().CountDocumentsAsync(filter);
        }

        public async Task<TEntity> Save<TEntity>(TEntity t) where TEntity : BaseEntity
        {
            await GetCollection<TEntity>().InsertOneAsync(t);
            return t;
        }

        public async Task<TEntity> Update<TEntity>(TEntity t) where TEntity : BaseEntity
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, t.Id);
            await GetCollection<TEntity>().ReplaceOneAsync(filter, t, new ReplaceOptions() { IsUpsert = false });
            return t;
        }

        public async Task<long> Delete<TEntity>(Expression<Func<TEntity, bool>> filter)
        {
            var d = await GetCollection<TEntity>().DeleteManyAsync(filter);
            return d.DeletedCount;
        }

        private MongoClient GetClient(string dbPath, bool showlog)
        {
            var mongoConnectionUrl = new MongoUrl(dbPath);
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoConnectionUrl);

            if (showlog)
            {
                mongoClientSettings.ClusterConfigurator = cb =>
                {
                    cb.Subscribe<CommandStartedEvent>(e =>
                    {
                        Log.ForContext<DbContext>().Debug($"[mongodb] {e.CommandName} - {e.Command.ToJson()}");
                    });
                };
            }

            return new MongoClient(mongoClientSettings);
        }
    }
}