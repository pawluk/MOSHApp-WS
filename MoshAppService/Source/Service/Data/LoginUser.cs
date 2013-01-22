// Project: MoshAppService
// Filename: LoginUser.cs
// 
// Author: Jason Recillo

using System;

using JetBrains.Annotations;

namespace MoshAppService.Service.Data {
    public class LoginUser : User {
        [NotNull]
        public string LoginName { get; set; }

        [NotNull]
        public string Password { get; set; }

        public LoginUser()
            : this(new User()) { }

        public LoginUser(User user, string pass = "")
            : base(user) {
            Password = pass;
        }

        public override string ToString() {
            // Strip "Password" field from output by representing this
            // object as a User
            return FromLoginUser(this).ToString();
        }
    }
}
