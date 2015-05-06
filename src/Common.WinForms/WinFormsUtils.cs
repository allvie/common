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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using JetBrains.Annotations;
using NanoByte.Common.Native;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides helper methods and API calls specific to the <see cref="System.Windows.Forms"/> UI toolkit.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static partial class WinFormsUtils
    {
        /// <summary>
        /// Forces a window to the foreground or flashes the taskbar if another process has the focus.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This method operates only on windows and not on individual controls.")]
        public static void SetForegroundWindow([NotNull] this Form form)
        {
            #region Sanity checks
            if (form == null) throw new ArgumentNullException("form");
            #endregion

            if (!WindowsUtils.IsWindows) return;
            UnsafeNativeMethods.SetForegroundWindow(form.Handle);
        }

        /// <summary>
        /// Adds a UAC shield icon to a button. Does nothing if not running Windows Vista or newer.
        /// </summary>
        /// <remarks>This is purely cosmetic. UAC elevation is a separate concern.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Native API only applies to buttons.")]
        public static void AddShieldIcon([NotNull] this Button button)
        {
            #region Sanity checks
            if (button == null) throw new ArgumentNullException("button");
            #endregion

            const int BCM_FIRST = 0x1600, BCM_SETSHIELD = 0x000C;

            if (!WindowsUtils.IsWindowsVista) return;
            button.FlatStyle = FlatStyle.System;
            UnsafeNativeMethods.SendMessage(button.Handle, BCM_FIRST + BCM_SETSHIELD, IntPtr.Zero, new IntPtr(1));
        }

        /// <summary>
        /// Determines whether <paramref name="key"/> is pressed right now.
        /// </summary>
        /// <remarks>Will always return <see langword="false"/> on non-Windows OSes.</remarks>
        public static bool IsKeyDown(Keys key)
        {
            if (WindowsUtils.IsWindows) return (SafeNativeMethods.GetAsyncKeyState((uint)key) & 0x8000) != 0;
            return false; // Not supported on non-Windows OSes
        }
    }
}