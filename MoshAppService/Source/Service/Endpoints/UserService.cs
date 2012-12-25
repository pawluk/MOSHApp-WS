// Project: MoshAppService
// Filename: UserService.cs
// 
// Author: Jason Recillo

using MoshAppService.Service.Data;
using MoshAppService.Service.Database;
using MoshAppService.Service.Response;

using ServiceStack.Logging;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.Text;

namespace MoshAppService.Service.Endpoints {
    public class UserService : MoshAppServiceBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UserService));

        public object Get(User request) {
            if (Session.Roles == null) return new UnauthorizedResponse();
            Log.Debug(Session.Dump());
            var userId = GetUserId(Session);

            if (userId != request.Id) return new UnauthorizedResponse();
            //TODO: Only let users retrieve information about their own profile, or their team member's profiles
            return UserDbProvider.GetUser(request.Id);
        }

        private static long GetUserId(IAuthSession session) {
            return long.Parse(session.Roles.Find(x => x.Contains("User")).Substring("User ".Length));
        }

        private static long GetTeamId(IAuthSession session) {
            return long.Parse(session.Roles.Find(x => x.Contains("Team")).Substring("Team ".Length));
        }
    }
}
