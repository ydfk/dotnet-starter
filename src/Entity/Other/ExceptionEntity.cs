//-----------------------------------------------------------------------
// <copyright file="ExceptionEntity.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 12:04:40</date>
//-----------------------------------------------------------------------

using FastHttpApi.Entity.Base;
using MongoDB.Bson.Serialization.Attributes;

namespace FastHttpApi.Entity.Other
{
    [BsonDiscriminator("exception")]
    [BsonIgnoreExtraElements]
    public class ExceptionEntity : BaseEntity
    {
        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string TargetSite { get; set; }
    }
}