// Project: MoshAppService
// Filename: MoshAppAuthProvider.cs
// 
// Author: Jason Recillo

using System.Collections.Generic;
using System.Globalization;

using MoshAppService.Service.Data;
using MoshAppService.Service.Database;
using MoshAppService.Utils;

using ServiceStack.Common;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.Text;

namespace MoshAppService.Service.Security {
    public class MoshAppAuthProvider : CredentialsAuthProvider {
        #region Possible User Roles

        /*
         * Upon logging in, each user will have these user roles attached
         * to it via the session's Roles property. These will then be used
         * later on to determine whether a given user should have access
         * to a given resource.
         */
        private const string User = "User {0}";
        private const string Team = "Team {0}";

        #endregion

        public override bool TryAuthenticate(IServiceBase authService, string userName, string password) {
            Log.Debug("TryAuthenticate({0}, {1})".F(userName, password));

            // No need to null-check user later on
            User user;
            if (!AuthenticateUser(userName, password, out user)) return false;

            var team = TeamDbProvider.GetTeam(user);

            var session = authService.GetSession();
            // PopulateWith() is really great, but it replaces the default 
            // session ID with the user ID from the database, so save it here
            var sessionId = session.Id;
            session.PopulateWith(user);
            session.Id = sessionId;
            session.IsAuthenticated = true;
            // These roles will be used later to check what team the user is on, etc.
            session.Roles = new List<string> {
                User.F(user.Id),
                Team.F(team.Id)
            };

            session.UserAuthName = userName;
            session.UserAuthId = user.Id.ToString(CultureInfo.InvariantCulture);

            Log.Debug(session.Dump());
            return true;
        }

        private static bool AuthenticateUser(string username, string password, out User user) {
            // Here, we would check the database and authenticate with the given credentials
            var login = LoginUserDbProvider.GetUser(username);

            // If user is null, the user doesn't exist in the database, so return false
            // Also, check the given password against the password retrieved from the database
            // and return false if it doesn't match
            if (login == null) {
                user = null;
                return false;
            }
            user = Data.User.FromLoginUser(login);
            return PasswordHelper.CheckPassword(password, login.Password);
        }
    }
}
