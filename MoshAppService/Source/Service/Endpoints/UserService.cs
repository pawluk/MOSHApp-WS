// Project: MoshAppService
// Filename: UserService.cs
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
using MoshAppService.Utils;

using MySql.Data.MySqlClient;

using ServiceStack.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.Text;

namespace MoshAppService.Service.Endpoints {
    [PublicAPI]
    public class UserService : MoshAppServiceBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UserService));

        public object Get(User request) {
            if (request.Id == -1 || !IsLoggedIn) return UnauthorizedResponse();

            // Only allow the user to see their own profile or the profile of anyone on their team
            var team = TeamDbProvider.Instance[TeamId];
            if (team != null && team.TeamMembers.Find(x => x.Id == request.Id) == null) return UnauthorizedResponse();

            return RequestContext.ToOptimizedResultUsingCache(Cache,
                                                              "User" + Session.Id + request.Id,
                                                              () => UserDbProvider.Instance[request.Id]);
        }

        public object Get(UserTaskDetails request) {
            if (request.UserId == -1) return BadRequestResponse();

            try {
                var response = new Dictionary<string, dynamic> { { "success", 0 }, { "error", 0 } };

                using (var conn = DbHelper.OpenConnection()) {
                    var cmd = new MySqlCommand {
                        Connection = conn,
                        CommandText = "GetTeamMemberDetails",
                        CommandType = CommandType.StoredProcedure,
                    };
                    cmd.Parameters.AddWithValue("UserId", request.UserId);

                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows) {
                        response["success"] = 1;
                        var i = 0;
                        while (reader.Read()) {
                            if (i == 0) {
                                response["nickname"] = reader.GetString("u_nickname");
                                response["tname"] = reader.GetString("t_name");
                            }
                            response.AddToDynamicList("tasks", new {
                                taskname = reader.GetString("tsk_name"),
                                taskstatus = reader.IsDBNull("status") ? 0 : reader.GetInt32("status"),
                                time_spent = reader.IsDBNull("time_spent") ? 0 : reader.GetInt64("time_spent"),
                            });
                            i++;
                        }
                    } else {
                        response["error"] = 1;
                        response["error_msg"] = "No team members found.";
                    }
                }

                return response;
            } catch (Exception e) {
                Log.Error(e.Message, e);
                throw;
            }
        }

        public object Get(UserOptions request) {
            if (request.UserId == -1 || !IsLoggedIn) return UnauthorizedResponse();

            var currentUser = UserDbProvider.Instance[UserId];
            request.UserId = UserId;
            request.PhoneVisible = currentUser.PhoneVisible;
            request.EmailVisible = currentUser.EmailVisible;

            return request;
        }

        public object Post(UserOptions request) {
            if (request.UserId == -1 || !IsLoggedIn) return UnauthorizedResponse();
            Log.Debug(request.Dump());

            UserDbProvider.Instance.UpdateUserOptions(request);

            return NoContentResponse();
        }
    }
}
