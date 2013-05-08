// Project: MoshAppService
// Filename: DynamicHelper.cs
// 
// Author: Jason Recillo

using System.Collections.Generic;
using System.Diagnostics;

namespace MoshAppService.Utils {
    public static class DynamicHelper {
        [DebuggerHidden]
        public static void AddToDynamicList(Dictionary<string, dynamic> dict, string key, dynamic obj) {
            if (dict.ContainsKey(key)) {
                dict[key].Add(obj);
            } else {
                dict[key] = new List<dynamic> { obj };
            }
        }
    }
}