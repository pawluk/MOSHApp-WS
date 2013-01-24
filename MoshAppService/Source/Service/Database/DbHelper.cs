// Project: MoshAppService
// Filename: DbHelper.cs
// 
// Author: Jason Recillo

using System;
using System.Configuration;

using MySql.Data.MySqlClient;

namespace MoshAppService.Service.Database {
    public static class DbHelper {
        public static MySqlConnection CreateConnection() {
            return new MySqlConnection(ConfigurationManager.ConnectionStrings["MoshConnection"].ConnectionString);
        }

        public static MySqlConnection OpenConnection() {
            var connection = CreateConnection();
            connection.Open();
            return connection;
        }

        public static void CloseConnection(MySqlConnection connection) {
            if (connection != null) connection.Close();
        }

        public static MySqlConnection OpenConnectionAndBeginTransaction(out MySqlTransaction transaction) {
            var connection = OpenConnection();
            transaction = connection.BeginTransaction();
            return connection;
        }

        public static void CloseConnectionAndEndTransaction(MySqlConnection connection, MySqlTransaction transaction, MySqlDataReader reader) {
            if (reader != null) reader.Close();
            if (transaction != null) transaction.Commit();
            CloseConnection(connection);
        }
    }
}
