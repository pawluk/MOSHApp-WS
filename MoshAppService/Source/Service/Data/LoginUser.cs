// Project: MoshAppService
// Filename: LoginUser.cs
// 
// Author: Jason Recillo

using System;

using JetBrains.Annotations;

namespace MoshAppService.Service.Data {
    public class LoginUser : User {
        [NotNull]
        public string Password { get; set; }

        public LoginUser(User user)
            : base(user) {
            Password = "";
        }

        public override string ToString() {
            // Strip "Password" field from output by representing this
            // object as a User
            return FromLoginUser(this).ToString();
        }
    }
}
