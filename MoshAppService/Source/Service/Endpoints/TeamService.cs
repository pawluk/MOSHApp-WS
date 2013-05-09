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

namespace MoshAppService.Service.Endpoints {
    [PublicAPI]
    public class TeamService : MoshAppServiceBase {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TeamService));

        public object Get(Team request) {
            if (request.Id == -1) return GetTeams(); // From PHP service

            if (!IsLoggedIn) return UnauthorizedResponse();

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
                            response.AddToDynamicList("contacts", new {
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

        public object Get(TeamMembers request) {
            if (request.TeamId == -1) return UnauthorizedResponse();

            try {
                using (var conn = DbHelper.OpenConnection()) {
                    var cmd = new MySqlCommand {
                        Connection = conn,
                        CommandText = "GetTeamMembers",
                        CommandType = CommandType.StoredProcedure,
                    };
                    cmd.Parameters.AddWithValue("TeamId", request.TeamId);

                    var response = new Dictionary<string, dynamic> { { "success", 0 }, { "error", 0 } };
                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows) {
                        response["success"] = 1;
                        var i = 0;
                        while (reader.Read()) {
                            if (i == 0) response["tname"] = reader.GetString("t_name");

                            response.AddToDynamicList("teammembers", new {
                                id = reader.GetInt64("u_id"),
                                nickname = reader.GetString("u_nickname"),
                                time_spent = reader.IsDBNull("time_spent") ? 0 : reader.GetInt64("time_spent"),
                            });
                            i++;
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

        private object GetTeams() {
            try {
                using (var conn = DbHelper.OpenConnection()) {
                    var cmd = new MySqlCommand {
                        Connection = conn,
                        CommandText = "GetTeams",
                        CommandType = CommandType.StoredProcedure,
                    };

                    var response = new Dictionary<string, dynamic> { { "success", 0 }, { "error", 0 } };
                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows) {
                        response["success"] = 1;
                        var i = 0;
                        var teams = new List<Dictionary<string, dynamic>>();
                        while (reader.Read()) {
                            var id = reader.GetInt64("t_id");
                            var tname = reader.GetString("t_name");
                            var timeSpent = reader.IsDBNull("time_spent") ? 0 : reader.GetInt64("time_spent");

                            Action add = () => teams.Add(new Dictionary<string, dynamic> {
                                { "id", id },
                                { "tname", tname },
                                { "time_spent", timeSpent },
                            });

                            if (i == 0) add();
                            else {
                                if (teams.Any(x => x["id"] == id)) teams.Single(x => x["id"] == id)["time_spent"] += timeSpent;
                                else add();
                            }
                            i++;
                        }
                        foreach (var t in teams) response.AddToDynamicList("teams", t);

                        // The original PHP service had two sort operations, this version has... this ಠ_ಠ
                        var teamList = ((List<dynamic>) response["teams"]);
                        var teamsWithTime = teamList.Where(x => x["time_spent"] != 0).ToList();
                        teamList.RemoveAll(x => teamsWithTime.Contains(x));
                        teamsWithTime.Sort((x, y) => x["time_spent"].CompareTo(y["time_spent"]));
                        response["teams"] = teamList.Prepend(teamsWithTime);
                    } else {
                        response["error"] = 1;
                        response["error_msg"] = "No teams found.";
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
