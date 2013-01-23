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

        private static readonly ILog Log = LogManager.GetLogger(typeof(TeamDbProvider));

        private const string BaseQuery = "SELECT " +
                                         "teams.*,users.* " +
                                         "FROM " +
                                         "users INNER JOIN " +
                                         "team_user ON team_user.u_id = users.u_id INNER JOIN " +
                                         "teams ON team_user.t_id = teams.t_id ";

        private const string SelectQuery = BaseQuery + "WHERE teams.t_id = @Id";
        private const string UserSelectQuery = BaseQuery + "WHERE users.u_id = @Id";

        protected override Team BuildObject(MySqlDataReader reader) {
            if (!reader.Read()) return null;

            var team = new Team {
                Id = reader.GetInt64("t_id"),
                Name = reader.GetString("t_name"),
                ChatId = reader.GetString("t_chat_id")
            };

            do {
                team.TeamMembers.Add(new User {
                    Id = reader.GetInt64("u_id"),
                    Nickname = reader.GetString("u_nicknme"),
                    FirstName = reader.GetString("u_fname"),
                    LastName = reader.GetString("u_lastname"),
                    Email = reader.GetString("u_email"),
                    Phone = reader.GetString("u_phone"),
                    StudentNumber = reader.GetString("s_num")
                });
            } while (reader.Read());

            return team;
        }

        public override Team this[long id] {
            get {
                MySqlTransaction tx;
                using (var conn = DbHelper.OpenConnectionAndBeginTransaction(out tx)) {
                    try {
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = SelectQuery;
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@Id", id);

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

        public Team this[User user] {
            get {
                using (var conn = DbHelper.OpenConnection()) {
                    var tx = conn.BeginTransaction();

                    var cmd = conn.CreateCommand();
                    cmd.CommandText = UserSelectQuery;
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Id", user.Id);

                    var reader = cmd.ExecuteReader();
                    var team = BuildObject(reader);

                    reader.Close();
                    tx.Commit();
                    conn.Close();

                    return team;
                }
            }
        }
    }
}
