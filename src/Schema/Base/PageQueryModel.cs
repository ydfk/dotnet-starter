//-----------------------------------------------------------------------
// <copyright file="PageQueryModel.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 17:47:29</date>
//-----------------------------------------------------------------------

namespace FastHttpApi.Schema.Base
{
    public class PageQueryModel
    {
        /// <summary>
        /// 一页的数量
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; } = 1;
    }
}