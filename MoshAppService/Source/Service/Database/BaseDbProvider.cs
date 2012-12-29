// Project: MoshAppService
// Filename: BaseDbProvider.cs
// 
// Author: Jason Recillo

using System;

using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;

namespace MoshAppService.Service.Database {
    public interface IDbProvider<out T> {
        //TODO: Connection-related stuff to the database will go here

        // Indexer (i.e., var item = dbProvider[id];)
        T this[long id] { get; }
    }

    public abstract class BaseDbProvider<T> : IDbProvider<T> {
        private static bool _initialized;

        protected static IDbConnectionFactory DbFactory {
            get {
                return new OrmLiteConnectionFactory(
                    @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\TempDb.mdf;Integrated Security=True;User Instance=True;",
                    true,
                    SqlServerOrmLiteDialectProvider.Instance);
            }
        }

        public BaseDbProvider() {
            InitDb();
        }

        private void InitDb() {
            if (_initialized) return;

            InitializeDb();
            _initialized = true;
        }

        protected abstract void InitializeDb();
        public abstract T this[long id] { get; }
    }
}
