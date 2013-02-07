// Project: MoshAppService
// Filename: TaskDict.cs
// 
// Author: Jason Recillo

using System;

namespace MoshAppService.Service.Data.Tasks {
    public class TaskDict : Entity<TaskDict> {
        #region Properties

        public string Directions { get; set; }
        public string AudioUrl { get; set; }
        public string ImageUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        #endregion

        #region Constructors

        public TaskDict()
            : this(-1, "", "", "", 0, 0) { }

        public TaskDict(long id, string d, string a, string i, double lat, double lon)
            : base(id) {
            Directions = d;
            AudioUrl = a;
            ImageUrl = i;
            Latitude = lat;
            Longitude = lon;
        }

        public TaskDict(TaskDict other)
            : this(other.Id,
                   other.Directions,
                   other.AudioUrl,
                   other.ImageUrl,
                   other.Latitude,
                   other.Longitude) { }

        #endregion

        #region Equality Members

        protected override bool _Equals(TaskDict other) {
            return Directions.Equals(other.Directions) &&
                   AudioUrl.Equals(other.AudioUrl) &&
                   ImageUrl.Equals(other.ImageUrl) &&
                   Math.Abs(Latitude - other.Latitude) < float.Epsilon &&
                   Math.Abs(Longitude - other.Longitude) < float.Epsilon;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Directions != null ? Directions.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AudioUrl != null ? AudioUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ImageUrl != null ? ImageUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Latitude.GetHashCode();
                hashCode = (hashCode * 397) ^ Longitude.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}
