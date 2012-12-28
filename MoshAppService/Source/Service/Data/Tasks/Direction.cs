// Project: MoshAppService
// Filename: Direction.cs
// 
// Author: Jason Recillo

using System;

using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Data.Tasks {
    public class Direction : Entity<Direction> {
        #region Properties

        public string Text { get; set; }
        public IFile Audio { get; set; }
        public IFile Image { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        #endregion

        #region Constructors

        public Direction()
            : this(-1, "", null, null, 0.0, 0.0) { }

        public Direction(long id, string text, IFile audio, IFile image, double lat, double lon)
            : base(id) {
            Text = text;
            Audio = audio;
            Image = image;
            Latitude = lat;
            Longitude = lon;
        }

        public Direction(Direction other)
            : this(other.Id,
                   other.Text,
                   other.Audio,
                   other.Image,
                   other.Latitude,
                   other.Longitude) { }

        #endregion

        #region Equality Members

        internal override bool _Equals(Direction other) {
            return string.Equals(Text, other.Text) &&
                   Equals(Audio, other.Audio) &&
                   Equals(Image, other.Image) &&
                   Latitude.Equals(other.Latitude) &&
                   Longitude.Equals(other.Longitude);
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

        #endregion
    }
}
