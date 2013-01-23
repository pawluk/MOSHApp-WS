// Project: MoshAppService
// Filename: Team.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override bool _Equals(Team other) {
            // For some reason this was returning false even when the other team's
            // team members are identical to this one, so this workaround will do.
            // The same situation will hold true for GetHashCode() as well.
            var x = string.Equals(Name, other.Name);
            x &= string.Equals(ChatId, other.ChatId);
            for (var i = 0; i < TeamMembers.Count; i++)
                x &= TeamMembers[i].Equals(other.TeamMembers[i]);
            return x;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ChatId != null ? ChatId.GetHashCode() : 0);
                if (TeamMembers != null) {
                    hashCode = TeamMembers.Aggregate(hashCode, (current, member) => (current * 397) ^ (member.GetHashCode()));
                }
                return hashCode;
            }
        }

        #endregion
    }
}
