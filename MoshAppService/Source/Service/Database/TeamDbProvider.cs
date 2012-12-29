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
using ServiceStack.OrmLite;

namespace MoshAppService.Service.Database {
    public class TeamDbProvider : BaseDbProvider<Team> {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<TeamDbProvider> _instance = new Lazy<TeamDbProvider>(() => new TeamDbProvider());
        public static TeamDbProvider Instance { get { return _instance.Value; } }
        private TeamDbProvider() { }

        #endregion

        private static readonly ILog Log = LogManager.GetLogger(typeof(TeamDbProvider));

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

        [PublicAPI]
        public static IEnumerable<Team> GetTeams(User user) {
            try {
                return Teams.Values.Where(x => x.TeamMembers.Contains(user));
            } catch (ArgumentNullException) {
                return null;
            }
        }

        protected override void InitializeDb() {
            using (var db = DbFactory.OpenDbConnection()) {
                db.DropTable<Team>();
                db.CreateTable<Team>();
                foreach (var team in Teams.Values) db.Insert(team);
            }
        }

        public override Team this[long id] {
            get {
                try {
                    using (var db = DbFactory.OpenDbConnection()) return db.GetById<Team>(id);
                } catch (InvalidOperationException) {
                    return null;
                }
            }
        }

        public Team this[User user] {
            get {
                try {
                    using (var db = DbFactory.OpenDbConnection())
                        return db.Select<Team>().Find(x => x.TeamMembers.Contains(user));
                } catch (ArgumentNullException) {
                    return null;
                } catch (InvalidOperationException) {
                    return null;
                }
            }
        }
    }
}
