/*
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
using NanoByte.Common.Values.Design;
using SlimDX;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Defines a three component vector with <see cref="double"/> accuracy.
    /// </summary>
    [TypeConverter(typeof(DoubleVector3Converter))]
    [StructLayout(LayoutKind.Sequential)]
    public struct DoubleVector3 : IEquatable<DoubleVector3>
    {
        #region Properties
        private double _x, _y, _z;

        /// <summary>
        /// Gets or sets the X component of the vector.
        /// </summary>
        [XmlAttribute, Description("Gets or sets the X component of the vector.")]
        public double X { get { return _x; } set { _x = value; } }

        /// <summary>
        /// Gets or sets the Y component of the vector. 
        /// </summary>
        [XmlAttribute, Description("Gets or sets the Y component of the vector.")]
        public double Y { get { return _y; } set { _y = value; } }

        /// <summary>
        /// Gets or sets the Z component of the vector.
        /// </summary>
        [XmlAttribute, Description("Gets or sets the Z component of the vector.")]
        public double Z { get { return _z; } set { _z = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new vector.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="z">The Z component.</param>
        public DoubleVector3(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }
        #endregion

        //--------------------//

        #region Add/Subtract with Vector3
        /// <summary>Add <see cref="DoubleVector3"/> to <see cref="Vector3"/></summary>
        public static DoubleVector3 operator +(DoubleVector3 vector1, Vector3 vector2)
        {
            return new DoubleVector3(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }

        /// <summary>Add <see cref="Vector3"/> to <see cref="DoubleVector3"/></summary>
        public static DoubleVector3 operator +(Vector3 vector1, DoubleVector3 vector2)
        {
            return new DoubleVector3(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }

        /// <summary>Subtract <see cref="DoubleVector3"/> from <see cref="Vector3"/></summary>
        public static DoubleVector3 operator -(DoubleVector3 vector1, Vector3 vector2)
        {
            return new DoubleVector3(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
        }

        /// <summary>Subtract <see cref="DoubleVector3"/> from <see cref="Vector3"/></summary>
        public static DoubleVector3 operator -(Vector3 vector1, DoubleVector3 vector2)
        {
            return new DoubleVector3(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
        }
        #endregion

        #region Add/Subtract with DoubleVector3 only
        /// <summary>Add <see cref="DoubleVector3"/> to <see cref="DoubleVector3"/></summary>
        public static DoubleVector3 operator +(DoubleVector3 vector1, DoubleVector3 vector2)
        {
            return new DoubleVector3(vector1._x + vector2._x, vector1._y + vector2._y, vector1._z + vector2._z);
        }

        /// <summary>Subtract <see cref="DoubleVector3"/> from <see cref="DoubleVector3"/></summary>
        public static DoubleVector3 operator -(DoubleVector3 vector1, DoubleVector3 vector2)
        {
            return new DoubleVector3(vector1._x - vector2._x, vector1._y - vector2._y, vector1._z - vector2._z);
        }

        /// <summary>
        /// Subtracts <paramref name="vector"/> from this and returns the result
        /// </summary>
        public DoubleVector3 Subtract(DoubleVector3 vector)
        {
            return this - vector;
        }
        #endregion

        #region Offset
        /// <summary>
        /// Returns a single-precision standard Vector3 after subtracting an offset value
        /// </summary>
        /// <param name="offset">This value is subtracting from the double-precision data before it is casted to single-precision</param>
        /// <returns>The relative value</returns>
        public Vector3 ApplyOffset(DoubleVector3 offset)
        {
            return new Vector3(
                (float)(_x - offset._x),
                (float)(_y - offset._y),
                (float)(_z - offset._z));
        }
        #endregion

        #region Scalar multiplication
        /// <summary>Multiply <see cref="DoubleVector3"/> with <see cref="double"/></summary>
        public static DoubleVector3 operator *(DoubleVector3 vector, double scalar)
        {
            var decScalar = scalar;
            return new DoubleVector3(vector._x * decScalar, vector._y * decScalar, vector._z * decScalar);
        }

        /// <summary>Multiply <see cref="DoubleVector3"/> with <see cref="double"/></summary>
        public static DoubleVector3 operator *(double scalar, DoubleVector3 vector)
        {
            var decScalar = scalar;
            return new DoubleVector3(vector._x * decScalar, vector._y * decScalar, vector._z * decScalar);
        }

        /// <summary>Multiply <see cref="DoubleVector3"/> with <see cref="float"/></summary>
        public static DoubleVector3 operator *(float scalar, DoubleVector3 vector)
        {
            var decScalar = scalar;
            return new DoubleVector3(vector._x * decScalar, vector._y * decScalar, vector._z * decScalar);
        }

        /// <summary>Multiply <see cref="DoubleVector3"/> with <see cref="float"/></summary>
        public static DoubleVector3 operator *(DoubleVector3 vector, float scalar)
        {
            var decScalar = scalar;
            return new DoubleVector3(vector._x * decScalar, vector._y * decScalar, vector._z * decScalar);
        }
        #endregion

        #region Dot product
        /// <summary>
        /// Calculates the dot product of this vector and <paramref name="vector"/>.
        /// </summary>
        /// <param name="vector">The second vector to calculate the dot product with.</param>
        /// <returns>this x <paramref name="vector"/></returns>
        public double DotProduct(DoubleVector3 vector)
        {
            return X * vector.X + Y * vector.Y + Z * vector.Z;
        }
        #endregion

        #region Length
        /// <summary>
        /// Calculates the length of the vector.
        /// </summary>
        public double Length()
        {
            return Math.Sqrt(_x * _x + _y * _y + _z * _z);
        }
        #endregion

        #region Flatten
        /// <summary>
        /// Maps X to X and Z to -Y. Drops Y.
        /// </summary>
        public Vector2 Flatten()
        {
            return new Vector2((float)X, (float)-Z);
        }
        #endregion

        //--------------------//

        #region Conversion
        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2})", X, Y, Z);
        }

        /// <summary>Convert <see cref="Vector3"/> into <see cref="DoubleVector3"/></summary>
        public static explicit operator DoubleVector3(Vector3 vector)
        {
            return new DoubleVector3(vector.X, vector.Y, vector.Z);
        }

        /// <summary>Convert <see cref="DoubleVector3"/> into <see cref="Vector3"/></summary>
        public static explicit operator Vector3(DoubleVector3 vector)
        {
            return new Vector3((float)vector._x, (float)vector._y, (float)vector._z);
        }
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(DoubleVector3 other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return other.X == X && other.Y == Y && other.Z == Z;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <inheritdoc/>
        public static bool operator ==(DoubleVector3 left, DoubleVector3 right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(DoubleVector3 left, DoubleVector3 right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DoubleVector3 && Equals((DoubleVector3)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 7;
                hash = 97 * hash + ((int)X ^ ((int)X >> 32));
                hash = 97 * hash + ((int)Y ^ ((int)Y >> 32));
                hash = 97 * hash + ((int)Z ^ ((int)Z >> 32));
                return hash;
            }
        }
        #endregion
    }
}
