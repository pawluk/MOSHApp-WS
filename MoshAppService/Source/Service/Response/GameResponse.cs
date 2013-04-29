// Project: MoshAppService
// Filename: GameResponse.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MoshAppService.Service.Data;

namespace MoshAppService.Service.Response {
    public class GameResponse : ResponseBase {
        public Game Game { get; set; }
    }
}
