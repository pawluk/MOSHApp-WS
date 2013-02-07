// Project: MoshAppService
// Filename: UserDbProvider.cs
// 
// Author: Jason Recillo

using System;

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

        public override User this[long id] {
            get {
                CheckIdIsValid(id);

                MySqlTransaction tx;
                MySqlDataReader reader = null;
                using (var conn = DbHelper.OpenConnectionAndBeginTransaction(out tx)) {
                    try {
                        var cmd = new MySqlCommand {
                            Connection = conn,
                            CommandText = Query
                        };
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("id", id);

                        reader = cmd.ExecuteReader();
                        var user = BuildObject(reader);

                        return user;
                    } finally {
                        DbHelper.CloseConnectionAndEndTransaction(conn, tx, reader);
                    }
                }
            }
        }

        protected override User BuildObject(MySqlDataReader reader) {
            if (!reader.Read() || !reader.HasRows) return null;
            return new User {
                Id = reader.GetInt64("u_id"),
                Nickname = reader.GetString("u_nickname"),
                FirstName = reader.GetString("u_fname"),
                LastName = reader.GetString("u_lastname"),
                Email = reader.GetString("u_email"),
                Phone = reader.GetString("u_phone"),
                StudentNumber = reader.GetString("s_num"),
                PhoneVisible = reader.GetBoolean("p_vsbl_tm"),
                EmailVisible = reader.GetBoolean("e_vsbl_tm")
            };
        }
    }
}
