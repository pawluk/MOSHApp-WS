// Project: MoshAppService
// Filename: BaseDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Diagnostics;

using MoshAppService.Service.Data;

using MySql.Data.MySqlClient;

namespace MoshAppService.Service.Database {
    public abstract class BaseDbProvider<T> : IDbProvider<T> where T : Entity<T> {
        public abstract T this[long id] { get; }
        protected abstract T BuildObject(MySqlDataReader reader);

        [DebuggerHidden]
        protected void CheckIdIsValid(long id) {
            if (id < 0) throw new ArgumentException("Id must be non-negative.", "id");
        }
    }
}
