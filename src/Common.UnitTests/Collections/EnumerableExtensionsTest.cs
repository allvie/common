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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="EnumerableExtensions"/>.
    /// </summary>
    public class EnumerableExtensionsTest
    {
        [Fact]
        public void TestMaxBy()
        {
            new[] {"abc", "a", "abcd", "ab"}.MaxBy(x => x.Length)
                .Should().Be("abcd");
        }

        [Fact]
        public void TestMaxByNull()
        {
            Enumerable.Empty<string>().MaxBy(x => x.Length)
                .Should().Be(null);
        }

        [Fact]
        public void TestMinBy()
        {
            new[] {"abc", "a", "abcd", "ab"}.MinBy(x => x.Length)
                .Should().Be("a");
        }

        [Fact]
        public void TestMinByNull()
        {
            Enumerable.Empty<string>().MaxBy(x => x.Length)
                .Should().Be(null);
        }

        [Fact]
        public void TestDistinctBy()
        {
            new[] {"a123", "a234", "b123"}.DistinctBy(x => x[0])
                .Should().BeEquivalentTo("a123", "b123");
        }

        [Fact]
        public void TestTrySelect()
        {
            var strings = new[] {"1", "2", "c", "4"};

            strings.TrySelect<string, int, FormatException>(int.Parse)
                .Should().Equal(1, 2, 4);

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            strings.Invoking(x => x.TrySelect<string, int, ArgumentException>(int.Parse).ToList())
                .ShouldThrow<FormatException>();
        }

        [Fact]
        public void TestAppend()
        {
            var strings = new List<string> {"A", "B"};
            strings.Append("C").Should().Equal("A", "B", "C");
        }

        [Fact]
        public void TestPrepend()
        {
            var strings = new List<string> {"B", "C"};
            strings.Prepend("A").Should().Equal("A", "B", "C");
        }

        private class Element
        {
            public readonly List<Element> Children = new List<Element>();
        }

        [Fact]
        public void TestTopologicalSort()
        {
            var b = new Element();
            var c = new Element {Children = {b}};
            var a = new Element {Children = {c}};
            var list = new List<Element> {a, b, c};

            list.TopologicalSort(getDependencies: x => x.Children)
                .Should().Equal(b, c, a);
        }

        [Fact]
        public async Task TestForEachAsync()
            => await new TaskFactory(CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskContinuationOptions.None, new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler)
                .StartNew(async () =>
                {
                    int runningTasksCount = 0;
                    await Enumerable.Range(1, 100).ForEachAsync(async x =>
                    {
                        runningTasksCount++;
                        await Task.Delay(10);
                        runningTasksCount.Should().BeLessOrEqualTo(2);
                        runningTasksCount--;
                    }, maxParallel: 2);
                }).Unwrap();

        [Fact]
        public async Task ForEachAsyncShouldRejectDefaultScheduler()
            => await Assert.ThrowsAsync<InvalidOperationException>(() => Enumerable.Range(1, 100).ForEachAsync(x => Task.CompletedTask));
    }
}
