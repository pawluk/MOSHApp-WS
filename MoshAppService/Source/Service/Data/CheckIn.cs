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
        public long QuestionId { get; set; }
        public string Response { get; set; }
        public string Location { get; set; }

        public CheckIn() {
            GameId = TaskId = QuestionId = -1;
            Response = Location = "";
        }
    }
}
