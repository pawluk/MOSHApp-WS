// Project: MoshAppService
// Filename: Campus.cs
// 
// Author: Jason Recillo

using System;

namespace MoshAppService.Service.Data.Tasks {
    public class Campus : Entity<Campus> {
        #region Properties

        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        #endregion

        #region Constructors

        public Campus()
            : this(-1, "", 0.0, 0.0) { }

        public Campus(long id, string name, double lat, double lon)
            : base(id) {
            Name = name;
            Latitude = lat;
            Longitude = lon;
        }

        public Campus(Campus other)
            : this(other.Id,
                   other.Name,
                   other.Latitude,
                   other.Longitude) { }

        #endregion

        #region Equality Members

        protected override bool _Equals(Campus other) {
            return string.Equals(Name, other.Name) &&
                   Latitude.Equals(other.Latitude) &&
                   Longitude.Equals(other.Longitude);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Latitude.GetHashCode();
                hashCode = (hashCode * 397) ^ Longitude.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}
