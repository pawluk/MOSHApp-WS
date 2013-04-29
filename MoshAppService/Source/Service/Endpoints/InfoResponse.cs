// Project: MoshAppService
// Filename: InfoResponse.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MoshAppService.Service.Response;

namespace MoshAppService.Service.Endpoints {
    public class InfoResponse : ResponseBase {
        public long UserId { get; set; }
        public long TeamId { get; set; }
        public long GameId { get; set; }
    }
}
