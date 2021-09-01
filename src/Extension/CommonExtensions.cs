//-----------------------------------------------------------------------
// <copyright file="CommonExtensions.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 13:50:30</date>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FastHttpApi.Extension
{
    public static class CommonExtensions
    {
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector)
        {
            return source.Distinct(new DistinctComparer<T, V>(keySelector));
        }

        public static Dictionary<string, string> ToDictionary(this object value)
        {
            var infos = value.GetType().GetProperties();
            var dix = new Dictionary<string, string>();

            foreach (var info in infos)
            {
                dix.Add(info.Name, info.GetValue(value)?.ToString());
            }

            return dix;
        }

        /// <summary>
        /// 为IEnumerable增加ForEach
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="iEnumerable">iEnumerable</param>
        /// <param name="method">method</param>
        public static void ForEach<T>(this IEnumerable<T> iEnumerable, Action<T> method)
        {
            iEnumerable.ToList().ForEach(method);
        }

        /// <summary>
        /// 为IEnumerable增加Any
        /// </summary>
        /// <param name="iEnumerable">iEnumerable</param>
        /// <returns>是否有值</returns>
        public static bool Any(this IEnumerable iEnumerable)
        {
            return iEnumerable != null && iEnumerable.Cast<object>().Any<object>();
        }

        /// <summary>
        /// 判断是空集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="iEnumerable">集合</param>
        /// <returns>是否为空</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> iEnumerable)
        {
            return iEnumerable == null || !iEnumerable.Any();
        }

        /// <summary>
        /// 判断不是空集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="iEnumerable">集合</param>
        /// <returns>是否为空</returns>
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> iEnumerable)
        {
            return !iEnumerable.IsNullOrEmpty();
        }

        /// <summary>
        /// 集合添加
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="item">item</param>
        public static void Add<T>(this ICollection<T> collection, T item)
        {
            if (collection != null && !object.Equals(item, default(T)))
            {
                collection.Add(item);
            }
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}