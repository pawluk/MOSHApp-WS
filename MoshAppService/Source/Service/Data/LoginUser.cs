﻿// Project: MoshAppService
// Filename: LoginUser.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JetBrains.Annotations;

namespace MoshAppService.Service.Data {
    public class LoginUser : User {
        [DebuggerHidden]
        public LoginUser()
            : this(new User()) { }

        [DebuggerHidden]
        public LoginUser(User user, string pass = "")
            : base(user) {
            Password = pass;
        }

        [NotNull]
        public string LoginName { get; set; }

        [NotNull]
        public string Password { get; set; }

        public override string ToString() {
            // Strip "Password" field from output by representing this
            // object as a User
            return FromLoginUser(this).ToString();
        }
    }
}
