// Project: MoshAppService
// Filename: MoshAppServiceBase.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;
using System.Net;

using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;

namespace MoshAppService.Service.Endpoints {
    public abstract class MoshAppServiceBase : ServiceStack.ServiceInterface.Service {
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

        public bool IsLoggedIn { get { return Session.IsAuthenticated; } }
        public long UserId { get { return long.Parse(Session.Roles.Find(x => x.Contains("User")).Substring("User ".Length)); } }
        public long TeamId { get { return long.Parse(Session.Roles.Find(x => x.Contains("Team")).Substring("Team ".Length)); } }

        protected HttpError UnauthorizedResponse(string errorCode = null) {
            return new HttpError(HttpStatusCode.Unauthorized, errorCode);
        }
    }
}
