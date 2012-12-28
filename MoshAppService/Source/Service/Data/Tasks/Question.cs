// Project: MoshAppService
// Filename: Question.cs
// 
// Author: Jason Recillo

using System;

namespace MoshAppService.Service.Data.Tasks {
    public class Question : Entity {
        public string Type { get; set; }
        public string CorrectAnswer { get; set; }

        protected bool Equals(Question other) {
            return base.Equals(other) &&
                   string.Equals(Type, other.Type) &&
                   string.Equals(CorrectAnswer, other.CorrectAnswer);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Question) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CorrectAnswer != null ? CorrectAnswer.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
