// Project: MoshAppService
// Filename: UserDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

using MoshAppService.Service.Data;
using MoshAppService.Service.Security;
using MoshAppService.Utils;

using MySql.Data.MySqlClient;

using ServiceStack.Text;

namespace MoshAppService.Service.Database {
    public class UserDbProvider : BaseDbProvider<User> {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<UserDbProvider> _instance = new Lazy<UserDbProvider>(() => new UserDbProvider());
        internal UserDbProvider() { }
        public static UserDbProvider Instance { get { return _instance.Value; } }

        #endregion

        private const string Query = "CALL GetUser(@id)";

        internal override User this[long id, MySqlConnection conn] {
            get {
                var cmd = new MySqlCommand {
                    Connection = conn,
                    CommandText = Query
                };
                cmd.Prepare();
                cmd.Parameters.AddWithValue("id", id);

                var reader = cmd.ExecuteReader();
                var user = BuildObject(reader);
                reader.Close();

                return user;
            }
        }

        protected override User BuildObject(MySqlDataReader reader) {
            if (!reader.Read() || !reader.HasRows) return null;
            return MakeUser(reader);
        }

        internal static User MakeUser(MySqlDataReader reader) {
            var user = new User {
                Id = reader.GetInt64("u_id"),
                Nickname = reader.GetString("u_nickname"),
                FirstName = reader.GetString("u_fname"),
                LastName = reader.GetString("u_lastname"),
                StudentNumber = reader.GetString("s_num"),
                Phone = null,
                Email = null,
                PhoneVisible = reader.GetBoolean("p_vsbl_tm"),
                EmailVisible = reader.GetBoolean("e_vsbl_tm")
            };
            if (user.PhoneVisible) user.Phone = reader.GetString("u_phone");
            if (user.EmailVisible) user.Email = reader.GetString("u_email");
            return user;
        }

        public void UpdateUserOptions(UserOptions opts) {
            MySqlTransaction tx = null;
            try {
                using (var conn = DbHelper.OpenConnectionAndBeginTransaction(out tx)) {
                    var cmd = new MySqlCommand {
                        Connection = conn,
                        CommandText = "UpdateUserOption",
                        CommandType = CommandType.StoredProcedure,
                    };
                    cmd.Parameters.AddWithValue("UserId", opts.UserId);
                    cmd.Parameters.AddWithValue("PhoneVisible", opts.PhoneVisible);
                    cmd.Parameters.AddWithValue("EmailVisible", opts.EmailVisible);

                    var rows = cmd.ExecuteNonQuery();

                    if (rows == 0) throw new ApplicationException("Error updating user options. Try again later.");

                    tx.Commit();
                }
            } catch (Exception e) {
                if (tx != null) {
                    try {
                        tx.Rollback();
                    } catch (InvalidOperationException ignored) { }
                }
                Log.Error(e.Message, e);
                throw;
            }
        }

        #region User Creation

        private static readonly char[] PasswordCharacters = "1234567890abcdefghjkmnpqrstuvwxyzACDEFGHJKLMNPQRSTUVWXYZ".ToCharArray();

        public void CreateUser(User obj, out dynamic credentials) {
            MySqlTransaction tx = null;
            try {
                using (var conn = DbHelper.OpenConnectionAndBeginTransaction(out tx)) {
                    var cmd = new MySqlCommand {
                        Connection = conn,
                        CommandText = "CreateUser",
                        CommandType = CommandType.StoredProcedure,
                    };
                    cmd.Parameters.AddWithValue("FirstName", obj.FirstName);
                    cmd.Parameters.AddWithValue("LastName", obj.LastName);
                    cmd.Parameters.AddWithValue("StudentNumber", obj.StudentNumber);
                    cmd.Parameters.AddWithValue("Phone", obj.Phone.IsNullOrEmpty() ? null : obj.Phone);
                    cmd.Parameters.AddWithValue("Email", obj.Email.IsNullOrEmpty() ? null : obj.Email); // Workaround to allow empty values when emails must be unique
                    cmd.Parameters.Add(new MySqlParameter {
                        ParameterName = "UserId",
                        Direction = ParameterDirection.Output,
                        MySqlDbType = MySqlDbType.Int32,
                    });

                    var rows = cmd.ExecuteNonQuery();
                    var userid = Convert.ToInt32(cmd.Parameters["UserId"].Value ?? -1);
                    if (rows == 0 || userid == -1) throw new ApplicationException("Could not create new user.");

                    string loginName, password;
                    GenerateUserCredentials(obj, out loginName, out password);

                    cmd = new MySqlCommand {
                        Connection = conn,
                        CommandText = "CreateLoginUser",
                        CommandType = CommandType.StoredProcedure,
                    };
                    cmd.Parameters.AddWithValue("UserId", userid);
                    cmd.Parameters.AddWithValue("LoginName", loginName);
                    cmd.Parameters.AddWithValue("Password", PasswordHelper.EncryptPassword(password));

                    rows = cmd.ExecuteNonQuery();
                    if (rows == 0) throw new ApplicationException("Could not create new user credentials.");

                    tx.Commit();
                    credentials = new {
                        loginName,
                        password
                    };
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

        [DebuggerHidden]
        private static bool IsLoginNameAvailable(string loginName) {
            try {
                using (var conn = DbHelper.OpenConnection()) {
                    var cmd = new MySqlCommand {
                        Connection = conn,
                        CommandText = "SELECT CheckLoginNameAvailable(@LoginName)",
                    };
                    cmd.Parameters.AddWithValue("LoginName", loginName);

                    var result = cmd.ExecuteScalar();

                    return Convert.ToBoolean(result ?? false);
                }
            } catch (Exception e) {
                Log.Error(e.Message, e);
                throw;
            }
        }

        [DebuggerHidden]
        private static string GenerateUsername(User user, string suffix = "") {
            return "{0}{1}{2}".Fmt(user.FirstName.Substring(0, 1), user.LastName, suffix).ToLower();
        }

        [DebuggerHidden]
        private static string GeneratePassword(int length) {
            var pb = new StringBuilder(length);
            for (var i = 0; i < length; i++)
                pb.Append(PasswordCharacters.GetRandom());
            return pb.ToString();
        }

        [DebuggerHidden]
        private static void GenerateUserCredentials(User user, out string username, out string password, int length = 6) {
            username = GenerateUsername(user);
            // Ensure that the username we generate for the new user is completely unique
            var count = 1;
            while (!IsLoginNameAvailable(username))
                username = GenerateUsername(user, "{0}".Fmt(count++));

            password = GeneratePassword(length);
        }

        #endregion
    }
}
