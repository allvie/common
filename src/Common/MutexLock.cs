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
using System.Threading;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides a wrapper around <see cref="Mutex"/> that automatically acquires on creating and releases on <see cref="Dispose"/>.
    /// </summary>
    /// <example>
    /// Instead of <c>lock (_object) { code(); }</c> for per-process locking use
    /// <c>using (new MutexLock("name") { code(); }</c> for inter-process locking.
    /// </example>
    /// <remarks>Automatically handles <see cref="AbandonedMutexException"/> with <see cref="Log.Warn(Exception)"/>.</remarks>
    public sealed class MutexLock : IDisposable
    {
        private readonly Mutex _mutex;

        /// <summary>
        /// Acquires <see cref="Mutex"/> with <paramref name="name"/>.
        /// </summary>
        public MutexLock(string name)
        {
            _mutex = new Mutex(false, name);
            try
            {
                _mutex.WaitOne();
            }
            catch (AbandonedMutexException ex)
            {
                // Abandoned mutexes also get aquired, but indicate something may have gone wrong elsewhere
                Log.Warn(ex);
            }
        }

        /// <summary>
        /// Releases the <see cref="Mutex"/>.
        /// </summary>
        public void Dispose()
        {
            try
            {
                _mutex.ReleaseMutex();
            }
            finally
            {
                _mutex.Close();
            }
        }
    }
}
