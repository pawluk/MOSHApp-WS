// Project: MoshAppService
// Filename: Init.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

using JetBrains.Annotations;

using MoshAppService.Service.Database;
using MoshAppService.Utils;

using MySql.Data.MySqlClient;

using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Endpoints {
    [Route("/init")]
    public class Init { }

    [PublicAPI]
    public class InitService : MoshAppServiceBase {
        public object Get(Init request) {
            if (!IsLoggedIn) return UnauthorizedResponse();

            //TODO: Extract this to own class
            try {
                using (var conn = DbHelper.OpenConnection()) {
                    var cmd = new MySqlCommand {
                        Connection = conn,
                        CommandText = "GetUserInitInfo",
                        CommandType = CommandType.StoredProcedure,
                    };
                    cmd.Parameters.AddWithValue("UserId", UserId);

                    var response = new Dictionary<string, dynamic> { { "success", 0 }, { "error", 0 } };
                    var reader = cmd.ExecuteReader();

                    using (var rows = new DataTable()) {
                        rows.Load(reader);
                        reader.Close();

                        // This code is ported (almost) directly from PHP to C#.
                        // As such, there is a lot of dynamically-typed code (using the dynamic keyword),
                        // calls to Convert.* methods, and uses of anonymous types instead of associative arrays.
                        // This implementation has been /slightly/ modified from its original (uncommented)
                        // PHP implementation, as I had some serious trouble understanding the original
                        // at some points and could not see why some things were done they way they were.
                        //
                        // Anyway, you have been warned. Confusing code ahoy!
                        if (rows.Rows.Count != 0) {
                            var number = rows.Rows.Count;
                            response["success"] = 1;
                            var count = 1;
                            var first = false;
                            var i = 0;
                            var prvdid = 0L;
                            var prvqid = "";

                            foreach (DataRow row in rows.Rows) {
                                // if( $row['status'] == 1 ) $first = true;
                                if (!IsNull(row, "status") && (Convert.ToInt32(row["status"]) == 1)) first = true;

                                if (first) {
                                    if (i == 0) {
                                        if (!IsNull(row, "status")) {
                                            response["userinfo"] = new {
                                                phoneoption = row["p_vsbl_tm"],
                                                emailoption = row["e_vsbl_tm"],
                                                teamid = row["t_id"],
                                                teamname = row["t_name"],
                                                gameid = row["g_id"],
                                                gamestart = row["start_time"],
                                                gamefinish = row["finis_time"],
                                                taskid = row["tsk_id"],
                                                tasksecret = row["secret_id"],
                                                taskname = row["tsk_name"],
                                                campusid = row["c_id"],
                                                campusname = row["c_name"],
                                                campuslat = row["c_lat"],
                                                campuslng = row["c_lng"],
                                            };
                                            response.AddToDynamicList("scripts", new {
                                                dictionaryid = row["td_id"],
                                                text = row["direction"],
                                                audio = row["audio"],
                                                image = row["image"],
                                                lat = row["td_lat"],
                                                lng = row["td_lng"],
                                            });

                                            string answer;
                                            if (!IsNull(row, "q_status") && Convert.ToInt32(row["q_status"]) == 1) {
                                                if (!IsNull(row, "answer")) {
                                                    answer = Convert.ToString(row["answer"]);
                                                } else {
                                                    answer = "No answer assigned";
                                                }
                                            } else {
                                                answer = "0";
                                            }

                                            response.AddToDynamicList("questions", new {
                                                questionid = row["q_id"],
                                                questiontype = row["q_typ_id"],
                                                question = row["q_text"],
                                                questionstatus = !IsNull(row, "q_status") ? row["q_status"] : 0,
                                                answer = answer,
                                            });
                                        } else {
                                            if (!IsNull(row, "g_id")) {
                                                response["userinfo"] = new {
                                                    phoneoption = row["p_vsbl_tm"],
                                                    emailoption = row["e_vsbl_tm"],
                                                    teamid = row["t_id"],
                                                    teamname = row["t_name"],
                                                    gameid = row["g_id"],
                                                    gamestart = row["start_time"],
                                                    gamefinish = row["finis_time"],
                                                };
                                            } else {
                                                response["userinfo"] = new {
                                                    phoneoption = row["p_vsbl_tm"],
                                                    emailoption = row["e_vsbl_tm"],
                                                };
                                            }
                                        }
                                    } else {
                                        if (!IsNull(row, "status")) {
                                            if (prvdid != Convert.ToInt64(row["td_id"])) {
                                                response.AddToDynamicList("scripts", new {
                                                    dictionaryid = row["td_id"],
                                                    text = row["direction"],
                                                    audio = row["audio"],
                                                    image = row["image"],
                                                    lat = row["td_lat"],
                                                    lng = row["td_lng"],
                                                });
                                            }

                                            var qsts = prvqid.Split('&');
                                            var found = false;
                                            for (var q = 0; q < qsts.Length; q++) {
                                                if (qsts[q] == Convert.ToString(row["q_id"])) {
                                                    found = true;
                                                    break;
                                                }
                                            }

                                            if (!found) {
                                                string answer;
                                                if (!row.IsNull("q_status") && Convert.ToInt32(row["q_status"]) == 1) {
                                                    if (!IsNull(row, "answer")) {
                                                        answer = Convert.ToString(row["answer"]);
                                                    } else {
                                                        answer = "No answer assigned.";
                                                    }
                                                } else {
                                                    answer = "0";
                                                }
                                                response.AddToDynamicList("questions", new {
                                                    questionid = row["q_id"],
                                                    questiontype = row["q_typ_id"],
                                                    question = row["q_text"],
                                                    questionstatus = !IsNull(row, "q_status") ? row["q_status"] : 0,
                                                    answer = answer,
                                                });
                                            }
                                        }
                                    }
                                    if (!IsNull(row, "status")) {
                                        prvdid = Convert.ToInt64(row["td_id"]);
                                        prvqid += Convert.ToString(row["q_id"]) + '&';
                                    }
                                    Global.Log.Debug(prvqid);
                                    i++; // Why this was being incremented here, I have no idea.
                                }
                                count++;
                                if (number == count && !first) {
                                    if (!IsNull(row, "g_id")) {
                                        response["userinfo"] = new {
                                            phoneoption = row["p_vsbl_tm"],
                                            emailoption = row["e_vsbl_tm"],
                                            teamid = row["t_id"],
                                            teamname = row["t_name"],
                                            gameid = row["g_id"],
                                            gamestart = row["start_time"],
                                            gamefinish = row["finis_time"],
                                        };
                                    } else {
                                        response["userinfo"] = new {
                                            phoneoption = row["p_vsbl_tm"],
                                            emailoption = row["e_vsbl_tm"],
                                        };
                                    }
                                }
                            }
                        } else {
                            response["error"] = 1;
                            response["error_msg"] = "There was an unknown error retrieving the information.";
                        }
                    }
                    // End ported PHP code. Whew, glad that's over!

                    return response;
                }
            } catch (Exception e) {
                Global.Log.Error(e.Message, e);
                throw;
            }
        }

        [DebuggerHidden]
        private static bool IsNull(DataRow row, string column) {
            return !(row.Table.Columns.Contains(column) && !row.IsNull(column));
        }
    }
}
