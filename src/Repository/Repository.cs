//-----------------------------------------------------------------------
// <copyright file="Repository.cs" company="QJJS">
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
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        public IMongoQueryable<TEntity> QueryData() => GetCollection().AsQueryable();

        public IMongoCollection<TEntity> GetCollection()
        {
            string tableName = BsonClassMap.LookupClassMap(typeof(TEntity)).Discriminator;
            var client = GetClient(AppSettings.MongoConnection, AppSettings.ShowMongoLog);
            var database = client.GetDatabase(AppSettings.MongoDatabase);
            return database.GetCollection<TEntity>(tableName);
        }

        public async Task<TModel> Get<TModel>(Expression<Func<TEntity, bool>> filter)
        {
            return (await GetCollection().Find(filter).SingleOrDefaultAsync()).Adapt<TModel>();
        }

        public async Task<IList<TModel>> List<TModel>(Expression<Func<TEntity, bool>> filter)
        {
            return (await GetCollection().Find(filter).ToListAsync()).Adapt<IList<TModel>>();
        }

        public async Task<IList<TModel>> List<TModel>()
        {
            return (await GetCollection().Find(Builders<TEntity>.Filter.Empty).ToListAsync()).Adapt<IList<TModel>>();
        }

        public async Task<IList<TModel>> ListByIds<TModel>(IList<string> ids)
        {
            var builderFilter = Builders<TEntity>.Filter;
            var query = builderFilter.In(x => x.Id, ids);

            return ((await GetCollection().FindAsync(query)).ToList()).Adapt<IList<TModel>>();
        }

        public PageResultModel<TModel> PageData<TModel>(Expression<Func<TEntity, bool>> filter, PageQueryModel pageQuery)
        {
            var pageResult = new PageResultModel<TModel>
            {
                TotalCount = QueryData().Count(),
                PageIndex = pageQuery.PageIndex,
                PageSize = pageQuery.PageSize,
            };

            List<TEntity> list;
            if (filter != null)
            {
                list = QueryData().Where(filter).Skip(pageQuery.PageSize * (pageQuery.PageIndex - 1)).Take(pageQuery.PageSize).ToList();
            }
            else
            {
                list = QueryData().Skip(pageQuery.PageSize * (pageQuery.PageIndex - 1)).Take(pageQuery.PageSize).ToList();
            }

            pageResult.Data = list.Adapt<IList<TModel>>();

            return pageResult;
        }

        public async Task<long> Count(Expression<Func<TEntity, bool>> filter)
        {
            return await GetCollection().CountDocumentsAsync(filter);
        }

        public async Task<TModel> Save<TModel>(TModel model)
        {
            var t = model.Adapt<TEntity>();
            t.DataStatus = true;
            t.CreateAt = DateTime.Now;
            t.UpdateAt = DateTime.Now;
            await GetCollection().InsertOneAsync(t);
            return t.Adapt<TModel>();
        }

        public async Task<TModel> Update<TModel>(TModel model)
        {
            var t = model.Adapt<TEntity>();
            t.DataStatus = true;
            t.UpdateAt = DateTime.Now;
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, t.Id);
            await GetCollection().ReplaceOneAsync(filter, t, new ReplaceOptions() { IsUpsert = false });
            return t.Adapt<TModel>();
        }

        public async Task<long> Delete(Expression<Func<TEntity, bool>> filter)
        {
            var d = await GetCollection().DeleteManyAsync(filter);
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
                        Log.ForContext<Repository<TEntity>>().Debug($"[mongodb] {e.CommandName} - {e.Command.ToJson()}");
                    });
                };
            }

            return new MongoClient(mongoClientSettings);
        }
    }
}