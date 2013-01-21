// Project: MoshAppService
// Filename: TeamService.cs
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
    public class TeamService : MoshAppServiceBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TeamService));

        public object Get(Team request) {
            Log.Debug(Session.Dump());
            if (request.Id == -1 || !IsLoggedIn) return UnauthorizedResponse();

            // Compare the user's team ID with the ID of the requested team
            if (TeamId != request.Id) return UnauthorizedResponse();

            //            var team = TeamDbProvider.Instance[request.Id];
            //            if (team == null) return BadRequestResponse();

            return RequestContext.ToOptimizedResultUsingCache(Cache,
                                                              "Team" + Session.Id + request.Id,
                                                              () => TeamDbProvider.Instance[request.Id]);
        }
    }
}
