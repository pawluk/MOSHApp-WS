// Project: MoshAppService
// Filename: ServiceExtensions.cs
// 
// Author: Jason Recillo

using System;

using ServiceStack.ServiceInterface.Auth;

namespace MoshAppService.Utils {
    public static class ServiceExtensions {
        public static string GetRole(this IAuthSession session, string role) {
            return session.Roles.Find(x => x.Contains(role));
        }
    }
}
