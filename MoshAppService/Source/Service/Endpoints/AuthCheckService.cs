// Project: MoshAppService
// Filename: AuthCheckService.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Endpoints {
    [Route("/authenticate/check", "GET")]
    public class AuthCheck { }

    public class AuthCheckService : MoshAppServiceBase {
        public object Get(AuthCheck request) {
            return IsLoggedIn;
        }
    }
}
