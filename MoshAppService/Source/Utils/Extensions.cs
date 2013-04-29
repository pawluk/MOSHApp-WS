// Project: MoshAppService
// Filename: Extensions.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MoshAppService.Utils {
    public static class Extensions {
        private static readonly Random Random = new Random();

        public static T GetRandom<T>(this IList<T> items) {
            var idx = Random.Next(0, items.Count - 1);
            return items[idx];
        }

        public static bool Empty<T>(this IList<T> arr) {
            return arr.Count == 0;
        }
    }
}
