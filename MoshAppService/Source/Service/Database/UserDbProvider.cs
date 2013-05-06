// Project: MoshAppService
// Filename: UserDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using MoshAppService.Service.Data;

using MySql.Data.MySqlClient;

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
    }
}
