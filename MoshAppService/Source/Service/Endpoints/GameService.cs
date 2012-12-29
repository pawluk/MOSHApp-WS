// Project: MoshAppService
// Filename: GameService.cs
// 
// Author: Jason Recillo

using System;

using JetBrains.Annotations;

using MoshAppService.Service.Data;
using MoshAppService.Service.Database;
using MoshAppService.Utils;

using ServiceStack.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.Text;

namespace MoshAppService.Service.Endpoints {
    public class GameService : MoshAppServiceBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameService));

        [PublicAPI]
        public object Get(Game request) {
            if (request.Id == -1 || !IsLoggedIn) return UnauthorizedResponse();
            return GameDbProvider.Instance[request.Id];
        }

        // Users check in at <service_url>/games/{gameId}/checkin,
        // providing the game ID in the url, and the task ID and answer in
        // either application/json or application/x-www-form-urlencoded format
        [PublicAPI]
        public object Post(CheckIn request) {
            Log.Debug("/games/{2}/checkin:{0}{1}".F(Environment.NewLine, request.Dump(), request.GameId));
            return null;
        }
    }
}
