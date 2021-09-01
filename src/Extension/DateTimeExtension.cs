//-----------------------------------------------------------------------
// <copyright file="DateTimeExtension.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 13:49:27</date>
//-----------------------------------------------------------------------
using System;
using System.Globalization;

namespace FastHttpApi.Extension
{
    public static class DateTimeExtension
    {
        public static long CurrentTimeMillis(this DateTime currentTime)
        {
            DateTime from = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (currentTime.Ticks - from.Ticks) / 10000;
        }

        /// <summary>
        /// 格式化日期，例如 2015-03-18
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>格式化后的字符串</returns>
        public static string DateFormat(this DateTime dt)
        {
            return (string.IsNullOrEmpty(dt.ToString(CultureInfo.InvariantCulture)) || dt == DateTime.MinValue) ? string.Empty : dt.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 格式化时间，例如 17:03:43
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>格式化后的字符串</returns>
        public static string TimeFormat(this DateTime dt)
        {
            return (string.IsNullOrEmpty(dt.ToString(CultureInfo.InvariantCulture)) || dt == DateTime.MinValue) ? string.Empty : dt.ToString("HH:mm:ss");
        }

        /// <summary>
        ///  格式化日期时间，例如 2015-03-18 17:03:43
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>格式化后的字符串</returns>
        public static string DateTimeFormat(this DateTime dt)
        {
            return (string.IsNullOrEmpty(dt.ToString(CultureInfo.InvariantCulture)) || dt == DateTime.MinValue) ? string.Empty : dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        ///  格式化日期时间，例如 20150318170343
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>格式化后的字符串</returns>
        public static string DateTimeNumberFormat(this DateTime dt)
        {
            return (string.IsNullOrEmpty(dt.ToString(CultureInfo.InvariantCulture)) || dt == DateTime.MinValue) ? string.Empty : dt.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        ///  格式化日期，例如 20150318
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>格式化后的字符串</returns>
        public static string MonthNumberFormat(this DateTime dt)
        {
            return (string.IsNullOrEmpty(dt.ToString(CultureInfo.InvariantCulture)) || dt == DateTime.MinValue) ? string.Empty : dt.ToString("yyyyMM");
        }
    }
}