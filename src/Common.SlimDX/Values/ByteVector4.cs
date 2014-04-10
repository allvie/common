﻿/*
 * Copyright 2006-2014 Bastian Eicher
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
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Defines a four component vector with <see cref="byte"/> accuracy.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ByteVector4 : IEquatable<ByteVector4>
    {
        #region Properties
        private byte _x, _y, _z, _w;

        /// <summary>
        /// Gets or sets the X component of the vector.
        /// </summary>
        [XmlAttribute, Description("Gets or sets the X component of the vector.")]
        public byte X { get { return _x; } set { _x = value; } }

        /// <summary>
        /// Gets or sets the Y component of the vector. 
        /// </summary>
        [XmlAttribute, Description("Gets or sets the Y component of the vector.")]
        public byte Y { get { return _y; } set { _y = value; } }

        /// <summary>
        /// Gets or sets the Z component of the vector.
        /// </summary>
        [XmlAttribute, Description("Gets or sets the Z component of the vector.")]
        public byte Z { get { return _z; } set { _z = value; } }

        /// <summary>
        /// Gets or sets the W component of the vector.
        /// </summary>
        [XmlAttribute, Description("Gets or sets the W component of the vector.")]
        public byte W { get { return _w; } set { _w = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new vector.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="z">The Z component.</param>
        /// <param name="w">The W component.</param>
        public ByteVector4(byte x, byte y, byte z, byte w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }
        #endregion

        //--------------------//

        #region Conversion
        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2}, {3})", X, Y, Z, W);
        }
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(ByteVector4 other)
        {
            return other.X == X && other.Y == Y && other.Z == Z && other.W == W;
        }

        /// <inheritdoc/>
        public static bool operator ==(ByteVector4 left, ByteVector4 right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(ByteVector4 left, ByteVector4 right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ByteVector4 && Equals((ByteVector4)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return X | (Y << 8) | (Z << 16) | (W << 24);
        }
        #endregion
    }
}
