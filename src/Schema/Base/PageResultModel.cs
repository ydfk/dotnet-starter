//-----------------------------------------------------------------------
// <copyright file="PageResultModel.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 17:41:46</date>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace FastHttpApi.Schema.Base
{
    public class PageResultModel<T>
    {
        /// <summary>
        /// 一页的数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IList<T> Data { get; set; }
    }
}