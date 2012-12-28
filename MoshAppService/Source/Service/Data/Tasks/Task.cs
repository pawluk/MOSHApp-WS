// Project: MoshAppService
// Filename: Task.cs
// 
// Author: Jason Recillo

using System;

namespace MoshAppService.Service.Data.Tasks {
    public class Task : Entity {
        public Campus Campus { get; set; }
        public Direction Direction { get; set; }
        public Question Question { get; set; }
        public Task Previous { get; set; }
    }

    public class Campus : Entity {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        protected bool Equals(Campus other) {
            return base.Equals(other) &&
                   string.Equals(Name, other.Name) &&
                   Latitude.Equals(other.Latitude) &&
                   Longitude.Equals(other.Longitude);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Campus) obj);
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
    }
}
