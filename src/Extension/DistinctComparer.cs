//-----------------------------------------------------------------------
// <copyright file="DistinctComparer.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 13:48:54</date>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace FastHttpApi.Extension
{
    public class DistinctComparer<T, V> : IEqualityComparer<T>
    {
        private readonly Func<T, V> _keySelector;

        public DistinctComparer(Func<T, V> keySelector)
        {
            this._keySelector = keySelector;
        }

        public bool Equals(T x, T y)
        {
            return EqualityComparer<V>.Default.Equals(_keySelector(x), _keySelector(y));
        }

        public int GetHashCode(T obj)
        {
            return EqualityComparer<V>.Default.GetHashCode(_keySelector(obj));
        }
    }
}