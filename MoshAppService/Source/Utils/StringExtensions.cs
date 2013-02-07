// Project: MoshAppService
// Filename: StringExtensions.cs
// 
// Author: Jason Recillo

using System;
using System.Diagnostics;

using JetBrains.Annotations;

namespace MoshAppService.Utils {
    public static class StringExtensions {
        [DebuggerHidden, NotNull, StringFormatMethod("str")]
        public static string F([NotNull] this string str, [NotNull] params object[] args) {
            return String.Format(str, args);
        }

        [DebuggerHidden]
        public static bool IsNullOrEmpty(this string input) {
            return string.IsNullOrEmpty(input);
        }

        [DebuggerHidden]
        public static bool IsNotNullOrEmpty(this string input) {
            return !string.IsNullOrEmpty(input);
        }
    }
}
