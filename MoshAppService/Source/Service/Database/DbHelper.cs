// Project: MoshAppService
// Filename: DbHelper.cs
// 
// Author: Jason Recillo

using System;
using System.Configuration;
using System.Diagnostics;

using MySql.Data.MySqlClient;

namespace MoshAppService.Service.Database {
    public static class DbHelper {
        [DebuggerHidden]
        public static MySqlConnection CreateConnection() {
            return new MySqlConnection(ConfigurationManager.ConnectionStrings["MoshConnection"].ConnectionString);
        }

        [DebuggerHidden]
        public static MySqlConnection OpenConnection() {
            var connection = CreateConnection();
            connection.Open();
            return connection;
        }

        [DebuggerHidden]
        public static void CloseConnection(MySqlConnection connection) {
            if (connection != null) connection.Close();
        }

        [DebuggerHidden]
        public static MySqlConnection OpenConnectionAndBeginTransaction(out MySqlTransaction transaction) {
            var connection = OpenConnection();
            transaction = connection.BeginTransaction();
            return connection;
        }

        [DebuggerHidden]
        public static void CloseConnectionAndEndTransaction(MySqlConnection connection, MySqlTransaction transaction, MySqlDataReader reader) {
            if (reader != null && !reader.IsClosed) reader.Close();
            if (transaction != null) transaction.Commit();
            CloseConnection(connection);
        }
    }
}
