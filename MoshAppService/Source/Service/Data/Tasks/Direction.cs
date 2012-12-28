// Project: MoshAppService
// Filename: Direction.cs
// 
// Author: Jason Recillo

using System;

using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Data.Tasks {
    public class Direction : Entity {
        public string Text { get; set; }
        public IFile Audio { get; set; }
        public IFile Image { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        protected bool Equals(Direction other) {
            return base.Equals(other) &&
                   string.Equals(Text, other.Text) &&
                   Equals(Audio, other.Audio) &&
                   Equals(Image, other.Image) &&
                   Latitude.Equals(other.Latitude) &&
                   Longitude.Equals(other.Longitude);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Direction) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Audio != null ? Audio.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Image != null ? Image.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Latitude.GetHashCode();
                hashCode = (hashCode * 397) ^ Longitude.GetHashCode();
                return hashCode;
            }
        }
    }
}
