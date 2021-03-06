﻿/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace NanoByte.Common.Values.Design
{
    /// <summary>
    /// Type converter for <see cref="Enum"/>s annotated with <see cref="XmlEnumAttribute"/>s.
    /// </summary>
    /// <typeparam name="T">The type the converter is used for.</typeparam>
    /// <example>
    ///   Add this attribute to the <see cref="Enum"/>:
    ///   <code>[TypeConverter(typeof(XmlEnumConverter&lt;NameOfEnum&gt;))]</code>
    /// </example>
    /// <remarks><see cref="XmlEnumAttribute.Name"/> is used as the case-insensitive string representation (falls back to element name).</remarks>
    public class EnumXmlConverter<T> : TypeConverter where T : struct
    {
        private static object GetEnumFromString(string stringValue)
        {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var field in typeof(T).GetFields())
            {
                var attributes = (XmlEnumAttribute[])field.GetCustomAttributes(typeof(XmlEnumAttribute), inherit: false);
                if (attributes.Length > 0 && StringUtils.EqualsIgnoreCase(attributes[0].Name, stringValue))
                    return field.GetValue(field.Name);
            }
            return Enum.Parse(typeof(T), stringValue, ignoreCase: true);
        }

        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            => value is string stringValue
                ? GetEnumFromString(stringValue)
                : base.ConvertFrom(context, culture, value);

        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            => value is Enum enumValue && destinationType == typeof(string)
            ? enumValue.GetEnumAttributeValue((XmlEnumAttribute attribute) => attribute.Name)
            : base.ConvertTo(context, culture, value, destinationType);

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

        private static readonly string[] _values = (from T value in Enum.GetValues(typeof(T)) select value.ConvertToString()).ToArray();

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) => new StandardValuesCollection(_values);
    }
}
