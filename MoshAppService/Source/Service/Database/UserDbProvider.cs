// Project: MoshAppService
// Filename: UserDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Data;
using System.Linq;

using MoshAppService.Service.Data;
using MoshAppService.Utils;

using MySql.Data.MySqlClient;

using NHibernate;
using NHibernate.Linq;

namespace MoshAppService.Service.Database {
    public class UserDbProvider : BaseDbProvider<User> {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<UserDbProvider> _instance = new Lazy<UserDbProvider>(() => new UserDbProvider());
        internal UserDbProvider() { }
        public static UserDbProvider Instance { get { return _instance.Value; } }

        #endregion

        public override User this[long id] {
            get {
                ITransaction tx;
                var session = NHibernateHelper.GetCurrentSessionAndStartTransaction(out tx);

                // In the current implementation, because LoginUser derives from User, it will also
                // appear in the query results (however all its fields will be blank), which is why
                // that in this current workaround, we have to do add a Single() call, in which we
                // check that the database fields marked NOT NULL are actually not null or empty.
                var user = session.Query<User>()
                                  .Where(x => x.Id == id).AsEnumerable()
                                  .Single(x => x.FirstName.IsNotNullOrEmpty() &&
                                               x.LastName.IsNotNullOrEmpty() &&
                                               x.StudentNumber.IsNotNullOrEmpty());

                NHibernateHelper.EndTransactionAndCloseSession(tx);
                return user as User;
            }
        }

        protected override User BuildObject(MySqlDataReader reader) {
            throw new NotImplementedException();
        }
    }
}
