// Project: MoshAppService
// Filename: ResponseBase.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using ServiceStack.ServiceInterface.ServiceModel;

namespace MoshAppService.Service.Response {
    [PublicAPI]
    public abstract class ResponseBase {
        [NotNull]
        [UsedImplicitly]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
