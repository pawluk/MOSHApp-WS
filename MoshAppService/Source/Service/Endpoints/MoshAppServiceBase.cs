// Project: MoshAppService
// Filename: MoshAppServiceBase.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;

namespace MoshAppService.Service.Endpoints {
    public abstract class MoshAppServiceBase : ServiceStack.ServiceInterface.Service {
        /// <summary>
        /// Obtains the user session from either the query string or a supplied cookie. If neither
        /// exists, create a new session.
        /// </summary>
        public new IAuthSession Session {
            get {
                try {
                    // If sessionId is not provided in the query string, check if a cookie was sent instead
                    return base.Cache.GetSession(Request.QueryString["sessionId"] ?? Request.Cookies["ss-id"].Value);
                } catch (KeyNotFoundException) {
                    return SessionFeature.GetOrCreateSession<AuthUserSession>(Cache);
                }
            }
        }

        /// <summary>
        /// Returns true if the user has successfully logged in.
        /// </summary>
        public bool IsLoggedIn { get { return Session.IsAuthenticated; } }

        /// <summary>
        /// Returns the user ID associated with the current session.
        /// </summary>
        public long UserId { get { return long.Parse(Session.Roles.Find(x => x.Contains("User")).Substring("User ".Length)); } }

        /// <summary>
        /// Returns the team ID of the team that the current user belongs to.
        /// </summary>
        public long TeamId { get { return long.Parse(Session.Roles.Find(x => x.Contains("Team")).Substring("Team ".Length)); } }

        /// <summary>
        /// Returns the game ID of the game that the current user is participating in.
        /// </summary>
        public long GameId { get { return long.Parse(Session.Roles.Find(x => x.Contains("Game")).Substring("Game ".Length)); } }

        protected HttpError UnauthorizedResponse(string errorCode = null) {
            return new HttpError(HttpStatusCode.Unauthorized, errorCode);
        }

        protected HttpError BadRequestResponse(string errorCode = null) {
            return new HttpError(HttpStatusCode.BadRequest, errorCode);
        }

        protected HttpResult OkResponse() {
            return new HttpResult(null, HttpStatusCode.OK);
        }

        protected HttpResult NoContentResponse() {
            return new HttpResult(null, HttpStatusCode.NoContent);
        }
    }
}
