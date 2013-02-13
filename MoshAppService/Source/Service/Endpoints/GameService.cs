﻿// Project: MoshAppService
// Filename: GameService.cs
// 
// Author: Jason Recillo

using System;
using System.Net;

using JetBrains.Annotations;

using MoshAppService.Service.Data;
using MoshAppService.Service.Database;
using MoshAppService.Utils;

using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.Text;

namespace MoshAppService.Service.Endpoints {
    public class GameService : MoshAppServiceBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameService));

        [PublicAPI]
        public object Get(Game request) {
            //            if (request.Id == -1 || !IsLoggedIn || request.Id != GameId) return UnauthorizedResponse();

            return RequestContext.ToOptimizedResultUsingCache(Cache,
                                                              "Game " + request.Id,
                                                              () => GameDbProvider.Instance[request.Id]);
        }

        // Users check in at <service_url>/games/{gameId}/checkin,
        // providing the game ID in the url, and the task ID and answer in
        // either application/json or application/x-www-form-urlencoded format
        [PublicAPI]
        public object Post(CheckIn request) {
            //TODO: Get game from database
            Log.Debug("/games/{1}/checkin:{0}{2}".F(Environment.NewLine, request.GameId, request.Dump()));
            Log.Debug("{0} has checked in".F(Cache.Get<User>("User {0}".F(UserId)).Nickname));

            var key = "Game " + request.GameId;
            if (Cache.Get<Game>(key) == null)
                Cache.Set(key, GameDbProvider.Instance[request.GameId]);

            // TODO: Check-in logic here

            RequestContext.RemoveFromCache(Cache, "Leaderboard");
            return new HttpResult(null, HttpStatusCode.NoContent);
        }
    }
}
