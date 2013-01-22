// Project: MoshAppService
// Filename: BaseDbProvider.cs
// 
// Author: Jason Recillo

using System;

using MoshAppService.Service.Data;

namespace MoshAppService.Service.Database {
    public interface IDbProvider<out T> where T : Entity<T> {
        // Indexer (i.e., var item = dbProvider[id];)
        T this[long id] { get; }
    }

    public abstract class BaseDbProvider<T> : IDbProvider<T> where T : Entity<T> {
        public abstract T this[long id] { get; }
    }
}
