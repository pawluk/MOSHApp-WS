﻿// Project: MoshAppService
// Filename: TaskDict.cs
// 
// Author: Jason Recillo

using System;

using MoshAppService.Units;

namespace MoshAppService.Service.Data.Tasks {
    public class TaskDict : Entity<TaskDict> {
        #region Properties

        public string Directions { get; set; }
        public string AudioUrl { get; set; }
        public string ImageUrl { get; set; }
        public Coordinate Location { get; set; }

        #endregion

        #region Constructors

        public TaskDict()
            : this(-1, "", "", "", new Coordinate()) { }

        public TaskDict(long id, string d, string a, string i, Coordinate c)
            : base(id) {
            Directions = d;
            AudioUrl = a;
            ImageUrl = i;
            Location = c;
        }

        public TaskDict(TaskDict other)
            : this(other.Id,
                   other.Directions,
                   other.AudioUrl,
                   other.ImageUrl,
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