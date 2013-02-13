// Project: MoshAppService
// Filename: TeamDbProvider.cs
// 
// Author: Jason Recillo

using System;

using MoshAppService.Service.Data;

using MySql.Data.MySqlClient;

using ServiceStack.Logging;

namespace MoshAppService.Service.Database {
    public class TeamDbProvider : BaseDbProvider<Team> {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<TeamDbProvider> _instance = new Lazy<TeamDbProvider>(() => new TeamDbProvider());
        private TeamDbProvider() { }
        public static TeamDbProvider Instance { get { return _instance.Value; } }

        #endregion

        // Select whole team (given a team ID)
        private const string SelectQuery = "CALL GetTeam(@id)";

        // Select whole team (given a team member's user ID)
        private const string UserSelectQuery = "CALL GetTeamWithUser(@id)";

        private static readonly ILog Log = LogManager.GetLogger(typeof(TeamDbProvider));

        internal override Team this[long id, MySqlConnection conn] {
            get {
                var cmd = new MySqlCommand {
                    Connection = conn,
                    CommandText = SelectQuery
                };
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@id", id);

                var reader = cmd.ExecuteReader();
                var team = BuildObject(reader);
                reader.Close();

                return team;
            }
        }

        public Team this[User user] {
            get {
                MySqlTransaction tx;
                using (var conn = DbHelper.OpenConnectionAndBeginTransaction(out tx)) {
                    try {
                        var cmd = new MySqlCommand {
                            Connection = conn,
                            CommandText = UserSelectQuery
                        };
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@id", user.Id);

                        var reader = cmd.ExecuteReader();
                        var team = BuildObject(reader);
                        reader.Close();

                        return team;
                    } finally {
                        DbHelper.CloseConnectionAndEndTransaction(conn, tx);
                    }
                }
            }
        }

        protected override Team BuildObject(MySqlDataReader reader) {
            if (!reader.Read() || !reader.HasRows) return null;

            var team = new Team {
                Id = reader.GetInt64("t_id"),
                Name = reader.GetString("t_name"),
                ChatId = reader.GetString("t_chat_id")
            };

            do {
                team.TeamMembers.Add(UserDbProvider.MakeUser(reader));
            } while (reader.Read());

            return team;
        }
    }
}
