// Project: MoshAppService
// Filename: GameDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MoshAppService.Service.Data;
using MoshAppService.Service.Data.Tasks;

using MySql.Data.MySqlClient;

using ServiceStack.Common;

namespace MoshAppService.Service.Database {
    public class GameDbProvider : BaseDbProvider<Game> {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<GameDbProvider> _instance = new Lazy<GameDbProvider>(() => new GameDbProvider());
        private GameDbProvider() { }
        public static GameDbProvider Instance { get { return _instance.Value; } }

        #endregion

        private const string QueryBase = "SELECT " +
                                         "  game.*, " +
                                         "  team_game.t_id " +
                                         "FROM " +
                                         "  game " +
                                         "    INNER JOIN team_game ON team_game.g_id = game.g_id " +
                                         "WHERE ";

        private const string GameQuery = QueryBase + "game.g_id = @id";

        private const string TeamQuery = QueryBase + "team_game.t_id = @id";

        private const string TaskQuery = "SELECT " +
                                         "  game_task.tsk_id " +
                                         "FROM " +
                                         "  game " +
                                         "    INNER JOIN game_task ON game_task.g_id = game.g_id " +
                                         "WHERE" +
                                         "  game.g_id = @id ";

        internal override Game this[long id, MySqlConnection conn] {
            get {
                var cmd = new MySqlCommand {
                    Connection = conn,
                    CommandText = GameQuery
                };
                cmd.Prepare();
                cmd.Parameters.AddWithValue("id", id);

                var reader = cmd.ExecuteReader();
                var game = BuildObject(reader);
                reader.Close();

                if (game == null) return null;

                game.Team.PopulateWith(TeamDbProvider.Instance[game.Team.Id, conn]);

                cmd = new MySqlCommand {
                    Connection = conn,
                    CommandText = TaskQuery
                };
                cmd.Prepare();
                cmd.Parameters.AddWithValue("id", id);

                reader = cmd.ExecuteReader();
                game.Tasks = BuildTasks(reader, game.Id);
                reader.Close();

                return game;
            }
        }

        private Game this[Team team, MySqlConnection conn] {
            get {
                var cmd = new MySqlCommand {
                    Connection = conn,
                    CommandText = TeamQuery
                };
                cmd.Prepare();
                cmd.Parameters.AddWithValue("id", team.Id);

                var reader = cmd.ExecuteReader();
                var game = BuildObject(reader);
                reader.Close();

                if (game == null) return null;

                if (team.IsInitialized) game.Team = team;
                else game.Team.PopulateWith(TeamDbProvider.Instance[team.Id, conn]);

                cmd = new MySqlCommand {
                    Connection = conn,
                    CommandText = TaskQuery
                };
                cmd.Prepare();
                cmd.Parameters.AddWithValue("id", game.Id);

                reader = cmd.ExecuteReader();
                game.Tasks = BuildTasks(reader, game.Id);
                reader.Close();

                return game;
            }
        }

        public Game this[Team team] {
            get {
                MySqlTransaction tx;
                using (var conn = DbHelper.OpenConnectionAndBeginTransaction(out tx)) {
                    try {
                        return this[team, conn];
                    } catch (Exception e) {
                        Log.Error(e.Message, e);
                        throw;
                    } finally {
                        DbHelper.CloseConnectionAndEndTransaction(conn, tx);
                    }
                }
            }
        }

        protected override Game BuildObject(MySqlDataReader reader) {
            if (!reader.Read()) return null;
            // Only initialize the team ID because we are going to get the rest of the object later
            return new Game {
                Id = reader.GetInt64("g_id"),
                Start = reader.GetDateTime("start_time"),
                Finish = reader.GetDateTime("finis_time"),
                Team = new Team { Id = reader.GetInt64("t_id") }
            };
        }

        private HashSet<Task> BuildTasks(MySqlDataReader reader, long gameId) {
            var tasks = new HashSet<Task>();

            while (reader.Read()) {
                var taskId = reader.GetInt64("tsk_id");
                tasks.Add(TaskDbProvider.Instance[taskId, gameId]);
            }

            return tasks;
        }
    }
}
