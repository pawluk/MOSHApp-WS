// Project: MoshAppService
// Filename: Campus.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MoshAppService.Units;

namespace MoshAppService.Service.Data.Tasks {
    public class Campus : Entity<Campus> {
        #region Properties

        public string Name { get; set; }
        public Coordinate Location { get; set; }

        #endregion

        #region Constructors

        public Campus()
            : this(-1, "", new Coordinate()) { }

        public Campus(long id, string name, Coordinate coord)
            : base(id) {
            Name = name;
            Location = coord;
        }

        public Campus(Campus other)
            : this(other.Id,
                   other.Name,
                   other.Location) { }

        #endregion

        #region Equality Members

        protected override bool _Equals(Campus other) {
            return string.Equals(Name, other.Name) &&
                   Location == other.Location;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Location.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}
