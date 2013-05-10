// Project: MoshAppService
// Filename: GameService.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using JetBrains.Annotations;

using MoshAppService.Service.Data;
using MoshAppService.Service.Database;

using MySql.Data.MySqlClient;

using ServiceStack.Logging;
using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Endpoints {
    public class GameService : MoshAppServiceBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameService));

        [PublicAPI]
        public object Get(Game request) {
            //            if (request.Id == -1 || !IsLoggedIn || request.Id != GameId) return UnauthorizedResponse();

            return RequestContext.ToOptimizedResultUsingCache(Cache,
                                                              "Game " + request.Id,
                                                              () => GameDbProvider.Instance[request.Id]);
        }

        // Users check in at <service_url>/games/{gameId}/checkin,
        // providing the game ID in the url, and the task ID and answer in
        // either application/json or application/x-www-form-urlencoded format
        [PublicAPI]
        public object Post(CheckIn checkIn) {
            if (checkIn.GameId != GameId || checkIn.TaskId == -1 || checkIn.QuestionId == -1 || !IsLoggedIn) return UnauthorizedResponse();

            MySqlTransaction tx = null;
            try {
                using (var conn = DbHelper.OpenConnectionAndBeginTransaction(out tx)) {
                    var response = new Dictionary<string, dynamic> { { "success", 0 }, { "error", 0 } };
                    if (checkIn.Response == "" && checkIn.Location != "") checkIn.Response = "My Picture";

                    response["test"] = checkIn.Response;
                    var cmd = new MySqlCommand {
                        Connection = conn,
                        CommandText = "InsertResponse",
                        CommandType = CommandType.StoredProcedure,
                    };
                    cmd.Parameters.AddWithValue("TeamId", TeamId);
                    cmd.Parameters.AddWithValue("UserId", UserId);
                    cmd.Parameters.AddWithValue("TaskId", checkIn.TaskId);
                    cmd.Parameters.AddWithValue("QuestionId", checkIn.QuestionId);
                    cmd.Parameters.AddWithValue("UResponse", checkIn.Response);
                    cmd.Parameters.AddWithValue("ULocation", checkIn.Location);

                    var addResponse = Convert.ToString(cmd.ExecuteScalar());

                    tx.Commit();

                    if (addResponse == "100") {
                        response["error"] = 1;
                        response["answered"] = 1;
                        response["error_msg"] = "You have already answered this question.";
                    } else {
                        if (addResponse != "0") {
                            switch (addResponse) {
                                case "gamecomplete":
                                    response["success"] = 1;
                                    response["gamecomplete"] = 1;
                                    break;
                                case "taskcomplete":
                                    return InitService.GetInitInfo(UserId);
                                default:
                                    response["success"] = 1;
                                    break;
                            }
                        } else {
                            response["error"] = 1;
                            response["error_msg"] = "An unknown error occurred while submitting your response.";
                        }
                    }
                    return response;
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
    }
}
