// Project: MoshAppService
// Filename: TeamDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using MoshAppService.Service.Data;

using ServiceStack.Logging;

namespace MoshAppService.Service.Database {
    public class TeamDbProvider : BaseDbProvider<Team> {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<TeamDbProvider> _instance = new Lazy<TeamDbProvider>(() => new TeamDbProvider());
        private TeamDbProvider() { }
        public static TeamDbProvider Instance { get { return _instance.Value; } }

        #endregion

        private static readonly ILog Log = LogManager.GetLogger(typeof(TeamDbProvider));

        #region Temporary in-memory "Database"

        internal static readonly Dictionary<long, Team> Teams = new Dictionary<long, Team> {
            {
                0, new Team {
                    Id = 0,
                    Name = "TTS",
                    TeamMembers = new List<User> {
                        UserDbProvider.Instance[0],
                        UserDbProvider.Instance[1],
                        UserDbProvider.Instance[4]
                    }
                }
            }, {
                1, new Team {
                    Id = 1,
                    Name = "SJS",
                    TeamMembers = new List<User> {
                        UserDbProvider.Instance[2],
                        UserDbProvider.Instance[3],
                        UserDbProvider.Instance[5]
                    }
                }
            }, {
                2, new Team {
                    Id = 2,
                    Name = "YHY",
                    TeamMembers = new List<User> {
                        UserDbProvider.Instance[6],
                        UserDbProvider.Instance[7],
                        UserDbProvider.Instance[8]
                    }
                }
            }
        };

        #endregion

        public override Team this[long id] {
            get {
                try {
                    return Teams[id];
                } catch (ArgumentException) {
                    return null;
                }
            }
        }

        public Team this[User user] { get { return Teams.ToList().Find(team => team.Value.TeamMembers.Contains(user)).Value; } }

        [PublicAPI]
        public static IEnumerable<Team> GetTeams(User user) {
            try {
                return Teams.Values.Where(x => x.TeamMembers.Contains(user));
            } catch (ArgumentNullException) {
                return null;
            }
        }
    }
}
