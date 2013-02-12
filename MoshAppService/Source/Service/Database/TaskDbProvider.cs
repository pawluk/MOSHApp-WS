// Project: MoshAppService
// Filename: TaskDbProvider.cs
// 
// Author: Jason Recillo

using System;

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

        private const string Query1 = "SELECT " +
                                      "  tasks.tsk_id, tasks.tsk_name, " +
                                      "  campus.c_id, campus.c_name, campus.c_lat, campus.c_lng, " +
                                      "  dic.td_id, dic.direction, dic.audio, dic.image, dic.td_lat, dic.td_lng " +
                                      "FROM " +
                                      "  tasks " +
                                      "    INNER JOIN campus ON tasks.c_id = campus.c_id " +
                                      "    INNER JOIN task_dic ON task_dic.tsk_id = tasks.tsk_id " +
                                      "    INNER JOIN dic ON task_dic.td_id = dic.td_id " +
                                      "WHERE " +
                                      "  tasks.tsk_id = @id ";

        private const string Query2 = "SELECT " +
                                      "  questions.q_id, questions.q_typ_id, questions.q_text " +
                                      "FROM " +
                                      "  questions " +
                                      "    INNER JOIN question_type ON questions.q_typ_id = question_type.q_typ_id " +
                                      "    INNER JOIN task_question ON task_question.q_id = questions.q_id " +
                                      "    INNER JOIN tasks ON task_question.tsk_id = tasks.tsk_id " +
                                      "WHERE " +
                                      "  tasks.tsk_id = @id ";

        public override Task this[long id] {
            get {
                CheckIdIsValid(id);

                MySqlTransaction tx;
                MySqlDataReader reader = null;
                using (var conn = DbHelper.OpenConnectionAndBeginTransaction(out tx)) {
                    try {
                        var cmd = new MySqlCommand {
                            Connection = conn,
                            CommandText = Query1
                        };
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("id", id);

                        reader = cmd.ExecuteReader();
                        var task = BuildObject(reader);
                        reader.Close();

                        // Continue building the questions half of the object with a second query.
                        // No, I don't like this implementation either. ):
                        // Maybe this could be one of the first major optimizations we could make?
                        cmd = new MySqlCommand {
                            Connection = conn,
                            CommandText = Query2
                        };
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("id", id);

                        reader = cmd.ExecuteReader();
                        task = BuildObject2(task, reader);

                        return task;
                    } finally {
                        DbHelper.CloseConnectionAndEndTransaction(conn, tx, reader);
                    }
                }
            }
        }

        protected override Task BuildObject(MySqlDataReader reader) {
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

        private Task BuildObject2(Task task, MySqlDataReader reader) {
            if (!reader.Read()) return task;

            do {
                task.Questions.Add(new Question {
                    Id = reader.GetInt64("q_id"),
                    QuestionText = reader.GetString("q_text"),
                    Type = (QuestionType) reader.GetInt32("q_typ_id")
                });
            } while (reader.Read());

            return task;
        }
    }
}
