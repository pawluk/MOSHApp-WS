// Project: MoshAppService
// Filename: LeaderboardService.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;

using MoshAppService.Service.Database;

using ServiceStack.Logging;
using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Endpoints {
    public class LeaderboardService : MoshAppServiceBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LeaderboardService));

        public object Get(Leaderboard request) {
            // Cache the leaderboard to avoid generating it multiple times, if 
            // the data hasn't changed. The leaderboard cache will be invalidated 
            // every time a player checks in, to keep the leaderboard accurate.
            return RequestContext.ToOptimizedResultUsingCache(Cache,
                                                              "Leaderboard",
                                                              GenerateLeaderboard);
        }

        public object Get(Leaderboard.LeaderboardUser request) {
            throw new NotImplementedException();
        }

        public object Get(Leaderboard.LeaderboardTeam request) {
            throw new NotImplementedException();
        }

        private static Leaderboard GenerateLeaderboard() {
            Log.Info("Generating leaderboard.");
            return new Leaderboard {
                Teams = new List<Leaderboard.LeaderboardTeam> {
                    Leaderboard.LeaderboardTeam.FromTeam(TeamDbProvider.Instance[0]),
                    Leaderboard.LeaderboardTeam.FromTeam(TeamDbProvider.Instance[1]),
                    Leaderboard.LeaderboardTeam.FromTeam(TeamDbProvider.Instance[2]),
                }
            };
        }
    }
}
