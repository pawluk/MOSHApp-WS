// Project: MoshAppService
// Filename: Coordinate.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MoshAppService.Utils;

namespace MoshAppService.Units {
    public struct Coordinate {
        public double Latitude;
        public double Longitude;

        public Coordinate(double lat, double lon)
            : this() {
            Latitude = lat;
            Longitude = lon;
        }

        public static Coordinate ParseJson(string json) {
            var c = json.Split(',');
            return new Coordinate {
                Latitude = double.Parse(c[0]),
                Longitude = double.Parse(c[1])
            };
        }

        public override string ToString() {
            return "{0},{1}".F(Latitude, Longitude);
        }

        #region Equality Members

        public bool Equals(Coordinate other) {
            return Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Coordinate && Equals((Coordinate) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (Latitude.GetHashCode() * 397) ^ Longitude.GetHashCode();
            }
        }

        public static bool operator ==(Coordinate a, Coordinate b) {
            return Math.Abs(a.Latitude - b.Latitude) < float.Epsilon &&
                   Math.Abs(a.Longitude - b.Longitude) < float.Epsilon;
        }

        public static bool operator !=(Coordinate a, Coordinate b) {
            return !(a == b);
        }

        #endregion
    }
}
