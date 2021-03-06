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
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using NanoByte.Common.Native;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Provides helper methods for the <see cref="System.Net"/> subsystem.
    /// </summary>
    public static class NetUtils
    {
        /// <summary>
        /// Applies environment variable HTTP proxy server configuration if present.
        /// </summary>
        /// <remarks>Uses classic Linux environment variables: http_proxy, http_proxy_user, http_proxy_pass</remarks>
        public static void ApplyProxy()
        {
            string httpProxy = Environment.GetEnvironmentVariable("http_proxy");
            string httpProxyUser = Environment.GetEnvironmentVariable("http_proxy_user");
            string httpProxyPass = Environment.GetEnvironmentVariable("http_proxy_pass");
            if (!string.IsNullOrEmpty(httpProxy))
            {
                WebRequest.DefaultWebProxy = string.IsNullOrEmpty(httpProxyUser)
                    ? new WebProxy(httpProxy)
                    : new WebProxy(httpProxy) {Credentials = new NetworkCredential(httpProxyUser, httpProxyPass)};
            }
        }

        /// <summary>
        /// Enables support for all available SSL/TLS versions.
        /// </summary>
        public static void ConfigureTls()
        {
            foreach (var protocol in Enum.GetValues(typeof(SecurityProtocolType)).Cast<SecurityProtocolType>())
            {
                try
                {
                    ServicePointManager.SecurityProtocol |= protocol;
                }
                catch (NotSupportedException)
                {
                    // Skip any unsupported protocols
                }
            }
        }

        /// <summary>
        /// Makes the SSL validation subsystem trust a set of certificates, even if their certificate chain is not trusted.
        /// </summary>
        /// <param name="publicKeys">The public keys of the certificates to trust.</param>
        /// <remarks>This method affects the global state of the <see cref="AppDomain"/>. Calling it more than once is not cumulative and will overwrite previous certificates. You should call this method exactly once near the beginning of your application.</remarks>
        public static void TrustCertificates([NotNull, ItemNotNull] params string[] publicKeys)
        {
            #region Sanity checks
            if (publicKeys == null) throw new ArgumentNullException(nameof(publicKeys));
            #endregion

            ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                if (sslPolicyErrors == SslPolicyErrors.None) return true;
                return (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors && publicKeys.Contains(certificate.GetPublicKeyString()));
            };
        }

        /// <summary>
        /// Determines whether an internet connection is currently available. May return false positives.
        /// </summary>
        public static bool IsInternetConnected => WindowsUtils.IsWindowsNT
            ? SafeNativeMethods.InternetGetConnectedState(out _, 0)
            : NetworkInterface.GetIsNetworkAvailable();

        [SuppressUnmanagedCodeSecurity]
        private static class SafeNativeMethods
        {
            [DllImport("wininet", SetLastError = true)]
            public static extern bool InternetGetConnectedState(out int lpdwFlags, int dwReserved);
        }
    }
}
