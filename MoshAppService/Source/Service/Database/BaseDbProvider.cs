// Project: MoshAppService
// Filename: BaseDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Diagnostics;

using MoshAppService.Service.Data;

using MySql.Data.MySqlClient;

using ServiceStack.Logging;

namespace MoshAppService.Service.Database {
    public abstract class BaseDbProvider<T> : IDbProvider<T> where T : Entity<T> {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(BaseDbProvider<T>));
        internal abstract T this[long id, MySqlConnection conn] { get; }

        public virtual T this[long id] {
            get {
                CheckIdIsValid(id);

                MySqlTransaction tx;
                using (var conn = DbHelper.OpenConnectionAndBeginTransaction(out tx)) {
                    try {
                        return this[id, conn];
                    } catch (Exception e) {
                        Log.Error(e.Message, e);
                        throw;
                    } finally {
                        DbHelper.CloseConnectionAndEndTransaction(conn, tx);
                    }
                }
            }
        }

        protected abstract T BuildObject(MySqlDataReader reader);

        [DebuggerHidden]
        protected void CheckIdIsValid(long id) {
            if (id < 0) throw new ArgumentException("Id must be non-negative.", "id");
        }
    }
}
