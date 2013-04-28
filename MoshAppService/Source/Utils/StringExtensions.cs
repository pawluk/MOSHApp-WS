// Project: MoshAppService
// Filename: StringExtensions.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

namespace MoshAppService.Utils {
    public static class StringExtensions {
        [DebuggerHidden, NotNull, StringFormatMethod("str")]
        public static string F([NotNull] this string str, [NotNull] params object[] args) {
            return String.Format(str, args);
        }

        [DebuggerHidden]
        public static string NormalizeSpaces(this string input) {
            return Regex.Replace(input, @"[ ]{2,}", " ");
        }

        [DebuggerHidden]
        public static string RemoveNewlines(this string input) {
            return Regex.Replace(input, @"\n", " ");
        }

        [DebuggerHidden]
        public static bool IsNullOrEmpty(this string input) {
            return string.IsNullOrEmpty(input);
        }

        [DebuggerHidden]
        public static bool IsNotNullOrEmpty(this string input) {
            return !string.IsNullOrEmpty(input);
        }

        [DebuggerHidden]
        public static bool ContainsAny(this IEnumerable<string> strs, bool caseSensitive, params string[] strsToCheck) {
            if (strsToCheck == null || strsToCheck.Empty()) throw new ArgumentNullException("strsToCheck");
            return caseSensitive ?
                       strsToCheck.Any(strs.Contains) :
                       strsToCheck.Any(str => strs.Select(x => x.ToLower()).Contains(str.ToLower()));
        }

        [DebuggerHidden]
        public static bool ContainsAll(this IEnumerable<string> strs, bool caseSensitive, params string[] strsToCheck) {
            if (strsToCheck == null || strsToCheck.Empty()) throw new ArgumentNullException("strsToCheck");
            return caseSensitive ?
                       strsToCheck.All(strs.Contains) :
                       strsToCheck.Any(str => strs.Select(x => x.ToLower()).Contains(str.ToLower()));
        }
    }
}
