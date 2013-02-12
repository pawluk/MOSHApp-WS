// Project: MoshAppService
// Filename: TaskDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;

using MoshAppService.Service.Data.Tasks;
using MoshAppService.Units;

using MySql.Data.MySqlClient;

namespace MoshAppService.Service.Database {
    public class TaskDbProvider : BaseDbProvider<Task> {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<TaskDbProvider> _instance = new Lazy<TaskDbProvider>(() => new TaskDbProvider());
        private TaskDbProvider() { }
        public static TaskDbProvider Instance { get { return _instance.Value; } }

        #endregion

        private const string TaskQuery = "SELECT " +
                                         "  tasks.tsk_id, tasks.tsk_name, tasks.secret_id, " +
                                         "  game_task.prv_tsk_id, " +
                                         "  dic.*, " +
                                         "  campus.* " +
                                         "FROM " +
                                         "  tasks " +
                                         "    INNER JOIN game_task ON game_task.tsk_id = tasks.tsk_id " +
                                         "    INNER JOIN task_dic ON task_dic.tsk_id = tasks.tsk_id " +
                                         "    INNER JOIN dic ON task_dic.td_id = dic.td_id " +
                                         "    INNER JOIN campus ON tasks.c_id = campus.c_id " +
                                         "WHERE " +
                                         "  tasks.tsk_id = @taskId AND" +
                                         "  game_task.g_id = @gameId ";

        private const string DictQuery = "SELECT " +
                                         "  questions.* " +
                                         "FROM " +
                                         "  dic " +
                                         "    INNER JOIN dic_question ON dic_question.td_id = dic.td_id " +
                                         "    INNER JOIN questions ON dic_question.q_id = questions.q_id " +
                                         "WHERE " +
                                         "  dic.td_id = @dictId ";

        /// <summary>
        /// Use TaskDbProvider[long taskid, long gameId] instead.
        /// </summary>
        [Obsolete]
        public override Task this[long id] { get { throw new NotSupportedException("Use TaskDbProvider[long taskid, long gameId] instead."); } }

        public Task this[long taskId, long gameId] {
            get {
                CheckIdIsValid(taskId);

                MySqlTransaction tx;
                MySqlDataReader reader = null;
                using (var conn = DbHelper.OpenConnectionAndBeginTransaction(out tx)) {
                    try {
                        var cmd = new MySqlCommand {
                            Connection = conn,
                            CommandText = TaskQuery
                        };
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("taskId", taskId);
                        cmd.Parameters.AddWithValue("gameId", gameId);

                        reader = cmd.ExecuteReader();
                        var task = BuildObject(reader);
                        reader.Close();

                        // Get all of the questions for each TaskDict
                        foreach (var dict in task.TaskDict) {
                            cmd = new MySqlCommand {
                                Connection = conn,
                                CommandText = DictQuery
                            };
                            cmd.Prepare();
                            cmd.Parameters.AddWithValue("dictId", dict.Id);

                            reader = cmd.ExecuteReader();
                            dict.Questions = BuildDictQuestion(reader);
                            reader.Close();
                        }
                        return task;
                    } finally {
                        DbHelper.CloseConnectionAndEndTransaction(conn, tx, reader);
                    }
                }
            }
        }

        protected override Task BuildObject(MySqlDataReader reader) {
            // Build the first half of the Task object.
            if (!reader.Read()) return null;
            var task = new Task {
                Id = reader.GetInt64("tsk_id"),
                Campus = new Campus {
                    Id = reader.GetInt64("c_id"),
                    Name = reader.GetString("c_name"),
                    Location = new Coordinate {
                        Latitude = reader.GetDouble("c_lat"),
                        Longitude = reader.GetDouble("c_lng")
                    }
                },
                Name = reader.GetString("tsk_name"),
            };
            // prv_tsk_id may be null, so only set a value here if it exists (default value is -1)
            if (reader.IsDBNull(reader.GetOrdinal("prv_tsk_id")))
                task.Previous = reader.GetInt64("prv_tsk_id");

            do {
                task.TaskDict.Add(new TaskDict {
                    Id = reader.GetInt64("td_id"),
                    Directions = reader.GetString("direction"),
                    AudioUrl = reader.GetString("audio"),
                    ImageUrl = reader.GetString("image"),
                    Location = new Coordinate {
                        Latitude = reader.GetDouble("td_lat"),
                        Longitude = reader.GetDouble("td_lng")
                    }
                });
            } while (reader.Read());

            return task;
        }

        private List<Question> BuildDictQuestion(MySqlDataReader reader) {
            // Build the second half of the Task object.
            if (!reader.Read()) return new List<Question>();

            var questions = new List<Question>();

            do {
                questions.Add(new Question {
                    Id = reader.GetInt64("q_id"),
                    QuestionText = reader.GetString("q_text"),
                    Type = (QuestionType) reader.GetInt32("q_typ_id")
                });
            } while (reader.Read());

            return questions;
        }
    }
}
