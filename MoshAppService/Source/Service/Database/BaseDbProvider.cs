// Project: MoshAppService
// Filename: BaseDbProvider.cs
// 
// Author: Jason Recillo

using System;

using MoshAppService.Service.Data;

using MySql.Data.MySqlClient;

namespace MoshAppService.Service.Database {
    public interface IDbProvider<out T> where T : Entity<T> {
        // Indexer (i.e., var item = dbProvider[id];)
        T this[long id] { get; }
    }

    public abstract class BaseDbProvider<T> : IDbProvider<T> where T : Entity<T> {
        public abstract T this[long id] { get; }
        protected abstract T BuildObject(MySqlDataReader reader);

        protected void CheckIdIsValid(long id) {
            if (id < 0) throw new ArgumentException("Id must be non-negative.", "id");
        }
    }
}
