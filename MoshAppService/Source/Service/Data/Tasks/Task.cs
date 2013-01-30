// Project: MoshAppService
// Filename: Task.cs
// 
// Author: Jason Recillo

using System;

using JetBrains.Annotations;

using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Data.Tasks {
    [PublicAPI]
    [Route("/tasks")]
    [Route("/tasks/{Id}", "GET")]
    public class Task : Entity<Task> {
        #region Properties

        public Campus Campus { get; set; }
        public Direction Direction { get; set; }
        public Question Question { get; set; }
        public long Previous { get; set; }

        #endregion

        #region Constructors

        public Task()
            : this(-1, new Campus(), new Direction(), new Question(), -1) { }

        public Task(long id, Campus c, Direction d, Question q, long prevId)
            : base(id) {
            Campus = c;
            Direction = d;
            Question = q;
            Previous = prevId;
        }

        public Task(long id, Campus c, Direction d, Question q, Task prev)
            : this(id, c, d, q, prev != null ? prev.Id : -1) { }

        public Task(Task other)
            : this(other.Id,
                   other.Campus,
                   other.Direction,
                   other.Question,
                   other.Previous) { }

        #endregion

        #region Equality Members

        protected override bool _Equals(Task other) {
            return Equals(Campus, other.Campus) &&
                   Equals(Direction, other.Direction) &&
                   Equals(Question, other.Question) &&
                   Equals(Previous, other.Previous);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Campus != null ? Campus.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Direction != null ? Direction.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Question != null ? Question.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Previous != null ? Previous.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }
}
