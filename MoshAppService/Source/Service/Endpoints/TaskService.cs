// Project: MoshAppService
// Filename: TaskService.cs
// 
// Author: Jason Recillo

using System;

using JetBrains.Annotations;

using MoshAppService.Service.Data.Tasks;
using MoshAppService.Service.Database;

using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Endpoints {
    public class TaskService : MoshAppServiceBase {
        // Users will be able to access /tasks to get all the tasks they have been assigned
        // Users can access individual tasks at /tasks/{id} as long as they have been assigned that task
        [PublicAPI]
        public object Get(Task request) {
            if (request.Id == -1 || !IsLoggedIn) return UnauthorizedResponse();
            // Only allow the user to see tasks with which they have been assigned

            return RequestContext.ToOptimizedResultUsingCache(Cache,
                                                              "Task" + Session.Id + request.Id + GameId,
                                                              () => TaskDbProvider.Instance[request.Id, GameId]);
        }
    }
}
