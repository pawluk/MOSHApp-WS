// Project: MoshAppService
// Filename: Game.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MoshAppService.Service.Data.Tasks;

using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Data {
    [PublicAPI]
    [Route("/games")]
    [Route("/games/{Id}", "GET")]
    public class Game : Entity<Game> {
        #region Properties

        public Team Team { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public IEnumerable<Task> Tasks { get; set; }

        #endregion

        #region Constructors

        public Game()
            : this(-1,
                   new Team(),
                   DateTime.Now,
                   DateTime.Now.AddHours(6.0),
                   new HashSet<Task>()) { }

        public Game(long id, Team team, DateTime start, DateTime finish, IEnumerable<Task> tasks)
            : base(id) {
            Team = team;
            Start = start;
            Finish = finish;
            Tasks = tasks;
        }

        public Game(Game other)
            : this(other.Id,
                   other.Team,
                   other.Start,
                   other.Finish,
                   other.Tasks) { }

        #endregion

        #region Equality Members

        protected override bool _Equals(Game other) {
            return Equals(Team, other.Team) &&
                   Start.Equals(other.Start) &&
                   Finish.Equals(other.Finish) &&
                   Equals(Tasks, other.Tasks);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Team != null ? Team.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Start.GetHashCode();
                hashCode = (hashCode * 397) ^ Finish.GetHashCode();
                hashCode = (hashCode * 397) ^ (Tasks != null ? Tasks.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }
}
