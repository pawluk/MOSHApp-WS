// Project: MoshAppService
// Filename: TeamDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using MoshAppService.Service.Data;

namespace MoshAppService.Service.Database {
    public class TeamDbProvider {
        #region Temporary in-memory "Database"

        internal static readonly Dictionary<long, Team> Teams = new Dictionary<long, Team> {
            {
                0, new Team {
                    Id = 0,
                    Name = "TTS",
                    TeamMembers = new List<User> {
                        UserDbProvider.Users[0],
                        UserDbProvider.Users[1],
                        UserDbProvider.Users[4]
                    }
                }
            }, {
                1, new Team {
                    Id = 1,
                    Name = "SJS",
                    TeamMembers = new List<User> {
                        UserDbProvider.Users[2],
                        UserDbProvider.Users[3],
                        UserDbProvider.Users[5]
                    }
                }
            }, {
                2, new Team {
                    Id = 2,
                    Name = "YHY",
                    TeamMembers = new List<User> {
                        UserDbProvider.Users[6],
                        UserDbProvider.Users[7],
                        UserDbProvider.Users[8]
                    }
                }
            }
        };

        #endregion

        [PublicAPI, CanBeNull]
        public static Team GetTeam(long teamId) {
            try {
                return Teams[teamId];
            } catch (InvalidOperationException) {
                return null;
            }
        }

        [PublicAPI]
        public static IEnumerable<Team> GetTeam(User user) {
            try {
                return Teams.Values.ToList().FindAll(x => x.TeamMembers.Contains(user));
            } catch (ArgumentNullException) {
                return null;
            }
        }
    }
}
