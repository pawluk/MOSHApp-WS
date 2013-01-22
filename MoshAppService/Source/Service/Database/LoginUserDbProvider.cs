// Project: MoshAppService
// Filename: LoginUserDbProvider.cs
// 
// Author: Jason Recillo

using System;

using JetBrains.Annotations;

using MoshAppService.Service.Data;

namespace MoshAppService.Service.Database {
    [UsedImplicitly]
    public class LoginUserDbProvider : UserDbProvider {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<LoginUserDbProvider> _instance = new Lazy<LoginUserDbProvider>(() => new LoginUserDbProvider());
        private LoginUserDbProvider() { }
        public new static LoginUserDbProvider Instance { get { return _instance.Value; } }

        #endregion

        public LoginUser this[string username] {
            get {
                if (username == null) throw new ArgumentNullException("username");

                var session = NHibernateHelper.GetCurrentSession();
                var tx = session.BeginTransaction();

                var query = session.CreateQuery("from LoginUser u where u.LoginName=:uname");
                query.SetString("uname", username);
                var user = query.UniqueResult<LoginUser>();

                tx.Commit();
                NHibernateHelper.CloseSession();
                return user;
            }
        }
    }
}
