// Project: MoshAppService
// Filename: LeaderboardService.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;
using System.Linq;

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

        [Route("/leaderboard/{TeamId}", "GET")]
        public class LeaderboardTeam {
            public long TeamId { get; set; }
            public string TeamName { get; set; }
            public TimeSpan TimeSpent { get; set; }
            public List<LeaderboardUser> TeamMembers { get; set; }

            public static LeaderboardTeam FromTeam(Team team) {
                var lt = new LeaderboardTeam {
                    TeamId = team.Id,
                    TeamName = team.Name,
                };
                lt.TeamMembers = new List<LeaderboardUser>(team.TeamMembers.Select(x => new LeaderboardUser(lt, x)));
                return lt;
            }
        }

        [Route("/leaderboard/{TeamId}/{UserId}", "GET")]
        public class LeaderboardUser {
            public long TeamId { get; set; }
            public long UserId { get; set; }
            public string TeamName { get; set; }
            public string Nickname { get; set; }
            public List<LeaderboardTask> Tasks { get; set; }

            public LeaderboardUser(LeaderboardTeam team, User user) {
                TeamId = team.TeamId;
                UserId = user.Id;
                TeamName = team.TeamName;
                Nickname = user.Nickname;
                Tasks = new List<LeaderboardTask>();
                // TODO: Set tasks
            }
        }

        public class LeaderboardTask {
            public string TaskName;
            public int TaskStatus;
            public TimeSpan TimeSpent;
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
