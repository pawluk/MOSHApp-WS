// Project: MoshAppService
// Filename: UserService.cs
// 
// Author: Jason Recillo

using JetBrains.Annotations;

using MoshAppService.Service.Data;
using MoshAppService.Service.Database;

using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.Text;

namespace MoshAppService.Service.Endpoints {
    [PublicAPI, Authenticate]
    public class UserService : MoshAppServiceBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UserService));

        public object Get(User request) {
            Log.Debug(Session.Dump());
            if (!IsLoggedIn) return UnauthorizedResponse();

            // Only allow the user to see their own profile or the profile of anyone on their team
            var team = TeamDbProvider.GetTeam(TeamId);
            if (team != null && team.TeamMembers.Find(x => x.Id == request.Id) == null) return UnauthorizedResponse();
            return UserDbProvider.GetUser(request.Id);
        }

        private static long GetUserId(IAuthSession session) {
            return GetId(session, "User");
        }

        private static long GetTeamId(IAuthSession session) {
            return GetId(session, "Team");
        }

        private static long GetId(IAuthSession session, string type) {
            return long.Parse(session.Roles.Find(x => x.Contains(type)).Substring(type.Length + 1));
        }
    }
}
