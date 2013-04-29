// Project: MoshAppService
// Filename: UserResponse.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MoshAppService.Service.Data;

namespace MoshAppService.Service.Response {
    public class UserResponse : ResponseBase {
        public User User { get; set; }
    }
}
