// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Web.Util
{
    internal static class HttpEncoderUtility
    {
        public static int HexToInt(char h) =>
            h >= '0' && h <= '9'
                ? h - '0'
                : h >= 'a' && h <= 'f'
                    ? h - 'a' + 10
                    : h >= 'A' && h <= 'F'
                        ? h - 'A' + 10
                        : -1;

        // Set of safe chars, from RFC 1738.4 minus '+'
        public static bool IsUrlSafeChar(char ch)
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
            {
                return true;
            }

            switch (ch)
            {
                case '-':
                case '_':
                case '.':
                case '!':
                case '*':
                case '(':
                case ')':
                    return true;
            }

            return false;
        }

        //  Helper to encode spaces only
        [return: NotNullIfNotNull("str")]
        internal static string? UrlEncodeSpaces(string? str) => str != null && str.Contains(' ') ? str.Replace(" ", "%20") : str;
    }
}
