// Project: MoshAppService
// Filename: TaskService.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using JetBrains.Annotations;

using MoshAppService.Service.Data.Tasks;
using MoshAppService.Service.Database;
using MoshAppService.Utils;

using MySql.Data.MySqlClient;

using ServiceStack.Logging;
using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Endpoints {
    public class TaskService : MoshAppServiceBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TaskService));

        // Users will be able to access /tasks to get all the tasks they have been assigned
        // Users can access individual tasks at /tasks/{id} as long as they have been assigned that task
        [PublicAPI]
        public object Get(Task request) {
            if (!IsLoggedIn) return UnauthorizedResponse();
            if (request.Id == -1) return GetTasks(); // From PHP service
            // Only allow the user to see tasks with which they have been assigned

            return RequestContext.ToOptimizedResultUsingCache(Cache,
                                                              "Task" + Session.Id + request.Id + GameId,
                                                              () => TaskDbProvider.Instance[request.Id, GameId]);
        }

        [PublicAPI]
        public object Post(Task request) {
            if (request.Id == -1 || !IsLoggedIn) return UnauthorizedResponse();

            MySqlTransaction tx = null;
            try {
                using (var conn = DbHelper.OpenConnectionAndBeginTransaction(out tx)) {
                    var cmd = new MySqlCommand {
                        Connection = conn,
                        CommandText = "AcceptTask",
                        CommandType = CommandType.StoredProcedure,
                    };
                    cmd.Parameters.AddWithValue("TeamId", TeamId);
                    cmd.Parameters.AddWithValue("TaskId", request.Id);
                    cmd.Parameters.AddWithValue("UserId", UserId);
                    cmd.Parameters.AddWithValue("Status", Request.FormData["status"]);

                    var success = cmd.ExecuteNonQuery() != 0;

                    if (success) tx.Commit();

                    return success ?
                               InitService.GetInitInfo(UserId) :
                               BadRequestResponse();
                }
            } catch (Exception e) {
                if (tx != null) {
                    try {
                        tx.Rollback();
                    } catch (InvalidOperationException) { }
                }
                Log.Error(e.Message, e);
                throw;
            }
        }

        [PublicAPI]
        public object Get(TaskDetail request) {
            if (request.TaskId == -1 || !IsLoggedIn) return UnauthorizedResponse();

            try {
                using (var conn = DbHelper.OpenConnection()) {
                    var cmd = new MySqlCommand {
                        Connection = conn,
                        CommandText = "GetTaskDetail",
                        CommandType = CommandType.StoredProcedure,
                    };
                    cmd.Parameters.AddWithValue("TaskId", request.TaskId);

                    var response = new Dictionary<string, dynamic> { { "success", 0 }, { "error", 0 } };
                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows) {
                        response["success"] = 1;
                        reader.Read();
                        var numDicts = 0;
                        var numQuestions = 0;
                        response["taskname"] = reader.GetString("tsk_name");
                        response["campus"] = reader.GetString("c_name");
                        response["campuslat"] = reader.GetDouble("c_lat");
                        response["campuslng"] = reader.GetDouble("c_lng");

                        do {
                            numQuestions += reader.GetInt32("questions");
                            numDicts++;
                            response["numberofdic"] = numDicts;
                            response["questions"] = numQuestions;
                        } while (reader.Read());
                    } else {
                        response["error"] = 1;
                        response["error_msg"] = "No tasks found.";
                    }

                    return response;
                }
            } catch (Exception e) {
                Log.Error(e.Message, e);
                throw;
            }
        }

        private object GetTasks() {
            try {
                using (var conn = DbHelper.OpenConnection()) {
                    var cmd = new MySqlCommand {
                        Connection = conn,
                        CommandText = "GetAllAvailableTasks",
                        CommandType = CommandType.StoredProcedure,
                    };
                    cmd.Parameters.AddWithValue("TeamId", TeamId);
                    cmd.Parameters.AddWithValue("UserId", UserId);
                    cmd.Parameters.AddWithValue("GameId", GameId);

                    var response = new Dictionary<string, dynamic> { { "success", 0 }, { "error", 0 } };
                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows) {
                        response["success"] = 1;
                        while (reader.Read()) {
                            response.AddToDynamicList("tasks", new {
                                requiredtsk = !reader.IsDBNull("prv_tsk_id") ? reader.GetString("prv_tsk_id") : "None",
                                taskid = reader.GetInt64("tsk_id"),
                                status = !reader.IsDBNull("status") ? reader.GetInt32("status") : 0,
                                user = !reader.IsDBNull("user") ? reader.GetString("user") : "0",
                                taskname = reader.GetString("tsk_name"),
                                campusid = reader.GetInt64("c_id"),
                                campusname = reader.GetString("c_name"),
                                campuslat = reader.GetDouble("c_lat"),
                                campuslng = reader.GetDouble("c_lng"),
                            });
                        }
                    } else {
                        response["error"] = 1;
                        response["error_msg"] = "No tasks found.";
                    }

                    return response;
                }
            } catch (Exception e) {
                Log.Error(e.Message, e);
                throw;
            }
        }
    }
}
