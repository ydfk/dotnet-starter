//-----------------------------------------------------------------------
// <copyright file="EnumExtension.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 13:48:04</date>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Reflection;

namespace FastHttpApi.Extension
{
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举变量值的 Description 属性
        /// </summary>
        /// <param name="obj">枚举变量</param>
        /// <returns>如果包含 Description 属性，则返回 Description 属性的值，否则返回枚举变量值的名称</returns>
        public static string GetDescription(this Enum obj)
        {
            return GetDescription(obj, false);
        }

        /// <summary>
        /// 获取枚举变量值的 Description 属性
        /// </summary>
        /// <param name="obj">枚举变量</param>
        /// <param name="isTop">是否改变为返回该类、枚举类型的头 Description 属性，而不是当前的属性或枚举变量值的 Description 属性</param>
        /// <returns>如果包含 Description 属性，则返回 Description 属性的值，否则返回枚举变量值的名称</returns>
        public static string GetDescription(this Enum obj, bool isTop)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            Type _enumType = obj.GetType();
            DescriptionAttribute dna = null;
            if (isTop)
            {
                dna = (DescriptionAttribute)Attribute.GetCustomAttribute(_enumType, typeof(DescriptionAttribute));
            }
            else
            {
                FieldInfo fi = _enumType.GetField(System.Enum.GetName(_enumType, obj));
                dna = (DescriptionAttribute)Attribute.GetCustomAttribute(
                  fi, typeof(DescriptionAttribute));
            }

            if (dna != null && !string.IsNullOrEmpty(dna.Description))
            {
                return dna.Description;
            }

            return obj.ToString();
        }

        /// <summary>
        /// 转换string为枚举
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="value">string</param>
        /// <returns>转换后的枚举</returns>
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}