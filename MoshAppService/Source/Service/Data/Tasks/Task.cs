// Project: MoshAppService
// Filename: Task.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Data.Tasks {
    [PublicAPI]
    [Route("/tasks")]
    [Route("/tasks/{Id}", "GET")]
    public class Task : Entity<Task> {
        #region Properties

        public string Name { get; set; }
        public Campus Campus { get; set; }
        public List<TaskDict> TaskDict { get; set; }
        public long Previous { get; set; }

        #endregion

        #region Constructors

        public Task()
            : this(-1, "", new Campus(), new List<TaskDict>(), -1) { }

        public Task(long id, string n, Campus c, List<TaskDict> dic, long prevId)
            : base(id) {
            Name = n;
            Campus = c;
            TaskDict = dic;
            Previous = prevId;
        }

        public Task(long id, string n, Campus c, List<TaskDict> dic, Task prev)
            : this(id, n, c, dic, prev != null ? prev.Id : -1) { }

        public Task(Task other)
            : this(other.Id,
                   other.Name,
                   other.Campus,
                   other.TaskDict,
                   other.Previous) { }

        #endregion

        #region Equality Members

        protected override bool _Equals(Task other) {
            return Equals(Campus, other.Campus) &&
                   Equals(TaskDict, other.TaskDict) &&
                   Equals(Previous, other.Previous);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Campus != null ? Campus.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TaskDict != null ? TaskDict.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Previous.GetHashCode());
                return hashCode;
            }
        }

        #endregion
    }

    [Route("/tasks/{TaskId}/detail")]
    public class TaskDetail {
        public long TaskId { get; set; }

        public TaskDetail() {
            TaskId = -1;
        }
    }
}
