/*
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
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// A string with an associated language that can be XML serialized to an element with an xml:lang tag.
    /// </summary>
    [Serializable]
    public sealed class LocalizableString : IEquatable<LocalizableString>, ICloneable<LocalizableString>
    {
        /// <summary>
        /// The default language: english with an invariant country.
        /// </summary>
        public static readonly CultureInfo DefaultLanguage = new CultureInfo("en");

        /// <summary>
        /// The actual string value to store.
        /// </summary>
        [Description("The actual string value to store.")]
        [XmlText]
        [CanBeNull]
        public string Value { get; set; }

        private CultureInfo _language = DefaultLanguage;

        /// <summary>
        /// The language of the <see cref="Value"/>.
        /// </summary>
        [Description("The language of the Value.")]
        [XmlIgnore]
        [NotNull]
        public CultureInfo Language
        {
            get { return _language; }
            set
            {
                #region Sanity checks
                if (value == null) throw new ArgumentNullException(nameof(value));
                #endregion

                _language = value.Equals(CultureInfo.InvariantCulture) ? DefaultLanguage : value;
            }
        }

        /// <summary>Used for XML serialization.</summary>
        /// <seealso cref="Language"/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        [XmlAttribute("lang", Namespace = "http://www.w3.org/XML/1998/namespace", DataType = "language") /* Will be serialized as xml:lang, must be done this way for Mono */]
        [CanBeNull]
        public string LanguageString
        {
            get => Language.ToString();
            set
            {
                try
                {
                    Language = string.IsNullOrEmpty(value)
                        // Default to English language
                        ? DefaultLanguage
                        // Handle Unix-style language codes (even though they are not actually valid in XML)
                        : new CultureInfo(value.Replace("_", "-"));
                }
                catch (ArgumentException)
                {
                    Log.Error("Ignoring unknown language code: " + value);
                }
            }
        }

        #region Conversion
        /// <inheritdoc/>
        public override string ToString() => Value + " (" + Language + ")";
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(LocalizableString other) => other != null && other.Value == Value && Language.Equals(other.Language);

        public static bool operator ==(LocalizableString left, LocalizableString right) => Equals(left, right);
        public static bool operator !=(LocalizableString left, LocalizableString right) => !Equals(left, right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is LocalizableString s && Equals(s);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = Language.GetHashCode();
                result = (result * 397) ^ Value?.GetHashCode() ?? 0;
                return result;
            }
        }
        #endregion

        #region Clone
        /// <summary>
        /// Creates a plain copy of this string.
        /// </summary>
        /// <returns>The cloned string.</returns>
        public LocalizableString Clone() => new LocalizableString {Language = Language, Value = Value};
        #endregion
    }
}
