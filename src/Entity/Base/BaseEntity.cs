//-----------------------------------------------------------------------
// <copyright file="BaseEntity.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 11:58:32</date>
//-----------------------------------------------------------------------
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace FastHttpApi.Entity.Base
{
    public class BaseEntity
    {
        protected BaseEntity()
        {
        }

        [BsonRequired]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string Id { get; set; }

        public virtual bool DataStatus { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public virtual DateTime CreateAt { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public virtual DateTime UpdateAt { get; set; }

        /// <summary>
        /// 额外元素，所有未包含在映射中元素会存在于此,类型可以为：IDictionary_string, object 或 BsonDocument
        /// </summary>
        [BsonExtraElements]
        public virtual IDictionary<string, object> CatchAll { get; set; }
    }
}