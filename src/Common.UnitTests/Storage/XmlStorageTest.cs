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

using FluentAssertions;
using Xunit;

#if SLIMDX
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
namespace NanoByte.Common.Storage.SlimDX
#else
namespace NanoByte.Common.Storage
#endif
{
    /// <summary>
    /// Contains test methods for <see cref="XmlStorage"/>.
    /// </summary>
    [Collection("WorkingDir")]
    public class XmlStorageTest
    {
        // ReSharper disable MemberCanBePrivate.Global
        /// <summary>
        /// A data-structure used to test serialization.
        /// </summary>
        [XmlNamespace("", "")]
        public class TestData
        {
            public string Data { get; set; }
        }
        // ReSharper restore MemberCanBePrivate.Global

        /// <summary>
        /// Ensures <see cref="XmlStorage.SaveXml{T}(T,string,string)"/> and <see cref="XmlStorage.LoadXml{T}(string)"/> work correctly.
        /// </summary>
        [Fact]
        public void TestFile()
        {
            TestData testData1 = new TestData {Data = "Hello"}, testData2;
            using (var tempFile = new TemporaryFile("unit-tests"))
            {
                // Write and read file
                testData1.SaveXml(tempFile);
                testData2 = XmlStorage.LoadXml<TestData>(tempFile);
            }

            // Ensure data stayed the same
            testData2.Data.Should().Be(testData1.Data);
        }

        /// <summary>
        /// Ensures <see cref="XmlStorage.SaveXml{T}(T,string,string)"/> and <see cref="XmlStorage.LoadXml{T}(string)"/> work correctly with relative paths.
        /// </summary>
        [Fact]
        public void TestFileRelative()
        {
            TestData testData1 = new TestData {Data = "Hello"}, testData2;
            using (new TemporaryWorkingDirectory("unit-tests"))
            {
                // Write and read file
                testData1.SaveXml("file.xml");
                testData2 = XmlStorage.LoadXml<TestData>("file.xml");
            }

            // Ensure data stayed the same
            testData2.Data.Should().Be(testData1.Data);
        }

        [Fact]
        public void TestToXmlString()
            => new TestData {Data = "Hello"}.ToXmlString().Should().Be("<?xml version=\"1.0\"?>\n<TestData>\n  <Data>Hello</Data>\n</TestData>\n");

        [Fact]
        public void TestFromXmlString()
            => XmlStorage.FromXmlString<TestData>("<?xml version=\"1.0\"?><TestData><Data>Hello</Data></TestData>").Data.Should().Be("Hello");

#if SLIMDX
        /// <summary>
        /// Ensures <see cref="XmlStorage.SaveXmlZip{T}(T,string,string,EmbeddedFile[])"/> and <see cref="XmlStorage.LoadXmlZip{T}(string,string,EmbeddedFile[])"/> work correctly with no password.
        /// </summary>
        [Fact]
        public void TestZipNoPassword()
        {
            // Write and read file
            var testData1 = new TestData {Data = "Hello"};
            var tempStream = new MemoryStream();
            testData1.SaveXmlZip(tempStream);
            tempStream.Seek(0, SeekOrigin.Begin);
            var testData2 = XmlStorage.LoadXmlZip<TestData>(tempStream);

            // Ensure data stayed the same
            testData2.Data.Should().Be(testData1.Data);
        }

        /// <summary>
        /// Ensures <see cref="XmlStorage.SaveXmlZip{T}(T,string,string,EmbeddedFile[])"/> and <see cref="XmlStorage.LoadXmlZip{T}(string,string,EmbeddedFile[])"/> work correctly with a password.
        /// </summary>
        [Fact]
        public void TestZipPassword()
        {
            // Write and read file
            var testData1 = new TestData {Data = "Hello"};
            var tempStream = new MemoryStream();
            testData1.SaveXmlZip(tempStream, "Test password");
            tempStream.Seek(0, SeekOrigin.Begin);
            var testData2 = XmlStorage.LoadXmlZip<TestData>(tempStream, password: "Test password");

            // Ensure data stayed the same
            testData2.Data.Should().Be(testData1.Data);
        }

        /// <summary>
        /// Ensures <see cref="XmlStorage.LoadXmlZip{T}(string,string,EmbeddedFile[])"/> correctly detects incorrect passwords.
        /// </summary>
        [Fact]
        public void TestIncorrectPassword()
        {
            var tempStream = new MemoryStream();
            var testData = new TestData {Data = "Hello"};
            testData.SaveXmlZip(tempStream, "Correct password");
            tempStream.Seek(0, SeekOrigin.Begin);
            Assert.Throws<ZipException>(() => XmlStorage.LoadXmlZip<TestData>(tempStream, password: "Wrong password"));
        }
#endif
    }
}
