// Project: MoshAppService
// Filename: TeamService.cs
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
    public class TeamService : MoshAppServiceBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TeamService));

        public object Get(Team request) {
            if (request.Id == -1 || !IsLoggedIn) return UnauthorizedResponse();

            // Compare the user's team ID with the ID of the requested team
            if (TeamId != request.Id) return UnauthorizedResponse();

            //            var team = TeamDbProvider.Instance[request.Id];
            //            if (team == null) return BadRequestResponse();

            return RequestContext.ToOptimizedResultUsingCache(Cache,
                                                              "Team" + Session.Id + request.Id,
                                                              () => TeamDbProvider.Instance[request.Id]);
        }

        public object Get(TeamContact request) {
            if (!IsLoggedIn) return UnauthorizedResponse();

            try {
                using (var conn = DbHelper.OpenConnection()) {
                    var cmd = new MySqlCommand {
                        Connection = conn,
                        CommandText = "GetContact",
                        CommandType = CommandType.StoredProcedure,
                    };
                    cmd.Parameters.AddWithValue("UserId", UserId);

                    var response = new Dictionary<string, dynamic> { { "success", 0 }, { "error", 0 } };
                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows) {
                        response["success"] = 1;
                        while (reader.Read()) {
                            DynamicHelper.AddToDynamicList(response, "contacts", new {
                                id = reader.GetInt64("u_id"),
                                nickname = reader.GetString("u_nickname"),
                                firstname = reader.GetString("u_fname"),
                                lastname = reader.GetString("u_lastname"),
                                email = reader.GetBoolean("e_vsbl_tm") ? reader.GetString("u_email") : null,
                                phone = reader.GetBoolean("p_vsbl_tm") ? reader.GetString("u_phone") : null,
                            });
                        }
                    } else {
                        response["error"] = 1;
                        response["error_msg"] = "No teams or teammates found.";
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
