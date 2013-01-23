// Project: MoshAppService
// Filename: Team.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Data {
    [PublicAPI]
    [Route("/teams")]
    [Route("/teams/{Id}", "GET")]
    public class Team : Entity<Team> {
        #region Properties

        public string Name { get; set; }
        // Populated using the Team_User table
        public List<User> TeamMembers { get; set; }
        public string ChatId { get; set; }

        #endregion

        #region Constructors

        public Team()
            : this(-1, "", new List<User>()) { }

        public Team(long id, string name, List<User> members)
            : base(id) {
            Name = name;
            TeamMembers = members;
        }

        public Team(Team other)
            : this(other.Id,
                   other.Name,
                   other.TeamMembers) { }

        #endregion

        #region Equality Members

        internal override bool _Equals(Team other) {
            return string.Equals(Name, other.Name) &&
                   Equals(TeamMembers, other.TeamMembers) &&
                   string.Equals(ChatId, other.ChatId);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TeamMembers != null ? TeamMembers.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }
}
