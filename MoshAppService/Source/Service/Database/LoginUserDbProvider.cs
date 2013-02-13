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

        private const string Query = "CALL GetLoginUser(@user)";
        private static readonly ILog Log = LogManager.GetLogger(typeof(LoginUserDbProvider));

        public LoginUser this[string username] {
            get {
                if (username == null) throw new ArgumentNullException("username");

                MySqlTransaction tx;
                using (var conn = DbHelper.OpenConnectionAndBeginTransaction(out tx)) {
                    try {
                        var cmd = new MySqlCommand {
                            Connection = conn,
                            CommandText = Query
                        };
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("user", username);

                        var reader = cmd.ExecuteReader();
                        var login = BuildObject(reader);
                        reader.Close();

                        return login;
                    } catch (MySqlException e) {
                        Log.Error(e.Message, e);
                        return null;
                    } catch (Exception e) {
                        Log.Error(e.Message, e);
                        throw;
                    } finally {
                        DbHelper.CloseConnectionAndEndTransaction(conn, tx);
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
