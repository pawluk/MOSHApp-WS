// Project: MoshAppService
// Filename: DynamicExtensions.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MoshAppService.Utils {
    // Where C#'s static type system comes to die
    public static class DynamicExtensions {
        [DebuggerHidden]
        public static void AddToDynamicList(this Dictionary<string, dynamic> dict, string key, dynamic obj) {
            if (dict.ContainsKey(key)) {
                dict[key].Add(obj);
            } else {
                dict[key] = new List<dynamic> { obj };
            }
        }
    }
}