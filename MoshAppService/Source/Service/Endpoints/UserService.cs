// Project: MoshAppService
// Filename: UserService.cs
// 
// Author: Jason Recillo

using System;

using JetBrains.Annotations;

using MoshAppService.Service.Data;
using MoshAppService.Service.Database;

using ServiceStack.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.Text;

namespace MoshAppService.Service.Endpoints {
    [PublicAPI, Authenticate]
    public class UserService : MoshAppServiceBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UserService));

        public object Get(User request) {
            Log.Debug(Session.Dump());
            if (request.Id == -1 || !IsLoggedIn) return UnauthorizedResponse();

            // Only allow the user to see their own profile or the profile of anyone on their team
            var team = TeamDbProvider.Instance[TeamId];
            if (team != null && team.TeamMembers.Find(x => x.Id == request.Id) == null) return UnauthorizedResponse();

            return RequestContext.ToOptimizedResultUsingCache(Cache,
                                                              "User" + Session.Id + request.Id,
                                                              () => UserDbProvider.Instance[request.Id]);
        }
    }
}
