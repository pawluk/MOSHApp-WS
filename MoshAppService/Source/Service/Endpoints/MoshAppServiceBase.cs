// Project: MoshAppService
// Filename: MoshAppServiceBase.cs
// 
// Author: Jason Recillo

using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;

namespace MoshAppService.Service.Endpoints {
    public abstract class MoshAppServiceBase : ServiceStack.ServiceInterface.Service {
        public new IAuthSession Session {
            get {
                // If sessionId is not provided in the query string, check if a cookie was sent instead
                return base.Cache.GetSession(Request.QueryString["sessionId"] ?? Request.Cookies["ss-id"].Value);
            }
        }
    }
}
