//-----------------------------------------------------------------------
// <copyright file="UserEntity.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 16:32:13</date>
//-----------------------------------------------------------------------

using FastDotnet.Entity.Base;
using MongoDB.Bson.Serialization.Attributes;

namespace FastDotnet.Entity.User
{
    [BsonDiscriminator("user")]
    public class UserEntity : BaseEntity
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}