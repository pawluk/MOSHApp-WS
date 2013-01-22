// Project: MoshAppService
// Filename: UserDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Linq;

using MoshAppService.Service.Data;

namespace MoshAppService.Service.Database {
    public class UserDbProvider : BaseDbProvider<User> {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<UserDbProvider> _instance = new Lazy<UserDbProvider>(() => new UserDbProvider());
        internal UserDbProvider() { }
        public static UserDbProvider Instance { get { return _instance.Value; } }

        #endregion

        public override User this[long id] {
            get {
                var session = NHibernateHelper.GetCurrentSession();
                var tx = session.BeginTransaction();

                var query = session.CreateQuery("from User u where u.Id=:id");
                query.SetInt64("id", id);

                // In the current implementation, because LoginUser derives from User,
                // it will also appear in the query results. In the current workaround,
                // we'll just filter out the results that aren't a User (there should
                // only really be two results anyway).
                var user = query.Enumerable<User>().ToList().Find(x => x.GetType() == typeof(User));
                // var user = query.UniqueResult<User>();

                tx.Commit();
                NHibernateHelper.CloseSession();
                return user;
            }
        }
    }
}
