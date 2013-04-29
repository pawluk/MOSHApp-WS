// Project: MoshAppService
// Filename: InfoService.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MoshAppService.Utils;

namespace MoshAppService.Service.Endpoints {
    public class InfoService : MoshAppServiceBase {
        public object Get(Info request) {
            if (!IsLoggedIn) return UnauthorizedResponse();

            return new InfoResponse {
                UserId = Convert.ToInt64(Session.GetRole("User").Substring(4)),
                TeamId = Convert.ToInt64(Session.GetRole("Team").Substring(4)),
                GameId = Convert.ToInt64(Session.GetRole("Game").Substring(4)),
            };
        }
    }
}
