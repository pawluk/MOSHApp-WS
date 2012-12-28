﻿// Project: MoshAppService
// Filename: ReflectionUtils.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MoshAppService.Utils {
    public static class ReflectionUtils {
        // http://stackoverflow.com/questions/506096/comparing-object-properties-in-c-sharp
        public static bool PublicInstancePropertiesEqual<T>(this T self, T to, params string[] ignore) where T : class {
            if (self != null && to != null) {
                var type = typeof(T);
                var ignoreList = new List<string>(ignore);
                var unequalProperties =
                    from pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    where !ignoreList.Contains(pi.Name)
                    let selfValue = type.GetProperty(pi.Name).GetValue(self, null)
                    let toValue = type.GetProperty(pi.Name).GetValue(to, null)
                    where selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue))
                    select selfValue;
                return !unequalProperties.Any();
            }
            return self == to;
        }
    }
}
