// Project: MoshAppService
// Filename: UnauthorizedResponse.cs
// 
// Author: Jason Recillo

using ServiceStack.ServiceInterface.ServiceModel;

namespace MoshAppService.Service.Response {
    public class UnauthorizedResponse : ResponseBase {
        public UnauthorizedResponse() {
            ResponseStatus = new ResponseStatus("401", "Unauthorized");
        }
    }
}
