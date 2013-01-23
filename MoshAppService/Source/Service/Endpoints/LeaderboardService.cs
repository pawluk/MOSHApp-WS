// Project: MoshAppService
// Filename: LeaderboardService.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MoshAppService.Service.Data;
using MoshAppService.Service.Database;

using ServiceStack.Logging;
using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Endpoints {
    [PublicAPI]
    [Route("/leaderboard", "GET")]
    public class Leaderboard {
        public List<LeaderboardTeam> Teams { get; set; }

        public Leaderboard() {
            Teams = new List<LeaderboardTeam>();
        }

        public class LeaderboardTeam : Entity<LeaderboardTeam> {
            public string TeamName { get; set; }
            public TimeSpan TimeSpent { get; set; }

            public static LeaderboardTeam FromTeam(Team team) {
                return new LeaderboardTeam {
                    Id = team.Id,
                    TeamName = team.Name
                };
            }

            internal override bool _Equals(LeaderboardTeam other) {
                return TeamName.Equals(other.TeamName) &&
                       TimeSpent.Equals(other.TimeSpent);
            }
        }
    }

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

