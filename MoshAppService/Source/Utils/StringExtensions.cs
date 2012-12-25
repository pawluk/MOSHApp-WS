// Project: MoshAppService
// Filename: StringExtensions.cs
// 
// Author: Jason Recillo

using System;

using JetBrains.Annotations;

namespace MoshAppService.Utils {
    public static class StringExtensions {
        [NotNull, StringFormatMethod("str")]
        public static string F([NotNull] this string str, [NotNull] params object[] args) {
            return String.Format(str, args);
        }
    }
}
