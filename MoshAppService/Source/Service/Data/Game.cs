// Project: MoshAppService
// Filename: Game.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;

using MoshAppService.Service.Data.Tasks;

namespace MoshAppService.Service.Data {
    public class Game : Entity {
        public Team Team { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public IEnumerable<Task> Tasks { get; set; }

        protected bool Equals(Game other) {
            return base.Equals(other) &&
                   Equals(Team, other.Team) &&
                   Start.Equals(other.Start) &&
                   Finish.Equals(other.Finish) &&
                   Equals(Tasks, other.Tasks);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Game) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Team != null ? Team.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Start.GetHashCode();
                hashCode = (hashCode * 397) ^ Finish.GetHashCode();
                hashCode = (hashCode * 397) ^ (Tasks != null ? Tasks.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
