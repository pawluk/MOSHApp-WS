// Project: MoshAppService
// Filename: LoginUserDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using MoshAppService.Service.Data;
using MoshAppService.Service.Security;

namespace MoshAppService.Service.Database {
    [UsedImplicitly]
    public class LoginUserDbProvider : UserDbProvider {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<LoginUserDbProvider> _instance = new Lazy<LoginUserDbProvider>(() => new LoginUserDbProvider());
        public new static LoginUserDbProvider Instance { get { return _instance.Value; } }
        private LoginUserDbProvider() { }

        #endregion

        #region Temporary in-memory "Database"

        private static readonly Dictionary<long, Tuple<string, string>> Passwords;

        static LoginUserDbProvider() {
            var storedpass = PasswordHelper.EncryptPassword("123456");

            // For now, just set all passwords to "123456".
            Passwords = new Dictionary<long, Tuple<string, string>>(Users.Count);

            foreach (var user in Users.Values) {
                // login name is first initial plus last name
                var login = (user.FirstName.Substring(0, 1) + user.LastName).ToLower();

                Passwords.Add(user.Id, new Tuple<string, string>(login, storedpass));
            }
        }

        #endregion

        protected override void InitializeDb() { }

        public LoginUser this[string username] {
            get {
                if (username == null) throw new ArgumentNullException("username");

                using (var db = DbFactory.OpenDbConnection()) {
                    try {
                        var userpass = Passwords.First(x => x.Value.Item1 == username);
                        var uid = userpass.Key;
                        var user = this[uid];
                        if (user == null) return null;

                        var loginUser = new LoginUser(user) {
                            Password = userpass.Value.Item2
                        };
                        return loginUser;
                    } catch (InvalidOperationException) {
                        return null;
                    }
                }
            }
        }
    }
}
