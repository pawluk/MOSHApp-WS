// Project: MoshAppService
// Filename: Authentication.cs
// 
// Author: Jason Recillo

using System.Linq;

using JetBrains.Annotations;

using MoshAppService.Service.Response;
using MoshAppService.Utils;

namespace MoshAppService.Service.Security {
    [PublicAPI]
    public class Authentication {
        public string Username { get; set; }
        public string Password { get; set; }

        public override string ToString() {
            // obfuscate password into asterisks
            var x = (Password ?? "").Aggregate("", (current, c) => current + "*");
            return "{0}:{1}".F(Username, x);
        }
    }

    public class AuthenticationResponse : ResponseBase {
        [UsedImplicitly]
        public string Result { get; set; }
    }
}
