// Project: MoshAppService
// Filename: TaskDict.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;

using MoshAppService.Units;
using MoshAppService.Utils;

namespace MoshAppService.Service.Data.Tasks {
    public class TaskDict : Entity<TaskDict> {
        private string _directions;

        #region Properties

        public string Directions { get { return _directions; } set { _directions = value.NormalizeSpaces(); } }
        public string AudioUrl { get; set; }
        public string ImageUrl { get; set; }
        public List<Question> Questions { get; set; }
        public Coordinate Location { get; set; }

        #endregion

        #region Constructors

        public TaskDict()
            : this(-1, "", "", "", new List<Question>(), new Coordinate()) { }

        public TaskDict(long id, string d, string a, string i,List<Question> q, Coordinate c)
            : base(id) {
            Directions = d;
            AudioUrl = a;
            ImageUrl = i;
            Questions = q;
            Location = c;
        }

        public TaskDict(TaskDict other)
            : this(other.Id,
                   other.Directions,
                   other.AudioUrl,
                   other.ImageUrl,
                   other.Questions,
                   other.Location) { }

        #endregion

        #region Equality Members

        protected override bool _Equals(TaskDict other) {
            return Directions.Equals(other.Directions) &&
                   AudioUrl.Equals(other.AudioUrl) &&
                   ImageUrl.Equals(other.ImageUrl) &&
                   Location.Equals(other.Location);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Directions != null ? Directions.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AudioUrl != null ? AudioUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ImageUrl != null ? ImageUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Location.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}
