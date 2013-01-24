// Project: MoshAppService
// Filename: LoginUserDbProvider.cs
// 
// Author: Jason Recillo

using System;

using JetBrains.Annotations;

using MoshAppService.Service.Data;

using MySql.Data.MySqlClient;

using ServiceStack.Logging;

namespace MoshAppService.Service.Database {
    [UsedImplicitly]
    public class LoginUserDbProvider : UserDbProvider {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<LoginUserDbProvider> _instance = new Lazy<LoginUserDbProvider>(() => new LoginUserDbProvider());
        private LoginUserDbProvider() { }
        public new static LoginUserDbProvider Instance { get { return _instance.Value; } }

        #endregion

        private static readonly ILog Log = LogManager.GetLogger(typeof(LoginUserDbProvider));

        private const string Query = "SELECT " +
                                     "users.*, login.login_name, login.login_pass " +
                                     "FROM " +
                                     "login INNER JOIN " +
                                     "users ON login.u_id = users.u_id " +
                                     "WHERE login.login_name = @user";

        public LoginUser this[string username] {
            get {
                if (username == null) throw new ArgumentNullException("username");

                MySqlTransaction tx;
                MySqlDataReader reader = null;
                using (var conn = DbHelper.OpenConnectionAndBeginTransaction(out tx)) {
                    try {
                        var cmd = new MySqlCommand {
                            Connection = conn,
                            CommandText = Query
                        };
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("user", username);

                        reader = cmd.ExecuteReader();
                        var login = BuildObject(reader);
                        return login;
                    } catch (MySqlException e) {
                        Log.Error(e.Message, e);
                        return null;
                    } finally {
                        DbHelper.CloseConnectionAndEndTransaction(conn, tx, reader);
                    }
                }
            }
        }

        protected new LoginUser BuildObject(MySqlDataReader reader) {
            if (!reader.HasRows) return null;
            return new LoginUser(base.BuildObject(reader)) {
                LoginName = reader.GetString("login_name"),
                Password = reader.GetString("login_pass")
            };
        }
    }
}
