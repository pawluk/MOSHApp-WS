// Project: MoshAppService
// Filename: Leaderboard.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using MoshAppService.Service.Data;

using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Endpoints {
    [PublicAPI]
    [Route("/leaderboard", "GET")]
    public class Leaderboard {
        public Leaderboard() {
            Teams = new List<LeaderboardTeam>();
        }

        public List<LeaderboardTeam> Teams { get; set; }

        public class LeaderboardTask {
            public string TaskName;
            public int TaskStatus;
            public TimeSpan TimeSpent;
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
            public LeaderboardUser(LeaderboardTeam team, User user) {
                TeamId = team.TeamId;
                UserId = user.Id;
                TeamName = team.TeamName;
                Nickname = user.Nickname;
                Tasks = new List<LeaderboardTask>();
                // TODO: Set tasks
            }

            public long TeamId { get; set; }
            public long UserId { get; set; }
            public string TeamName { get; set; }
            public string Nickname { get; set; }
            public List<LeaderboardTask> Tasks { get; set; }
        }
    }
}
