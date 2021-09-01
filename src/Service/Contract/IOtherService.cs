//-----------------------------------------------------------------------
// <copyright file="IOtherService.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 11:57:26</date>
//-----------------------------------------------------------------------
using FastHttpApi.Entity.Other;
using System.Threading.Tasks;

namespace FastHttpApi.Service.Contract
{
    public interface IOtherService
    {
        Task SaveException(ExceptionEntity exception);
    }
}