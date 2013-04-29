// Project: MoshAppService
// Filename: CheckIn.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Data {
    [PublicAPI]
    [Route("/games/{GameId}/checkin", "POST")]
    public class CheckIn {
        public long GameId { get; set; }
        public long TaskId { get; set; }
        public string Answer { get; set; }
    }
}
