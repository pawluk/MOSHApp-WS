// Project: MoshAppService
// Filename: Team.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using ServiceStack.ServiceHost;
using ServiceStack.Text;

namespace MoshAppService.Service.Data {
    [PublicAPI]
    [Route("/teams/{Id}")]
    public class Team : Entity {
        public string Name { get; set; }
        // Populated using the Team_User table
        public List<User> TeamMembers { get; set; }

        // What to do with these two fields?
        //        public long ChatId { get; set; }
        //        public string ChatUuid { get; set; }

        public override string ToString() {
            return JsonSerializer.SerializeToString(this);
        }

        protected bool Equals(Team other) {
            return base.Equals(other) &&
                   string.Equals(Name, other.Name) &&
                   Equals(TeamMembers, other.TeamMembers);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Team) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TeamMembers != null ? TeamMembers.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
