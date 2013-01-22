// Project: MoshAppService
// Filename: Question.cs
// 
// Author: Jason Recillo

using System;

namespace MoshAppService.Service.Data.Tasks {
    public class Question : Entity<Question> {
        #region Properties

        public QuestionType Type { get; set; }
        // Will only be set if QuestionType is Image or Sound
        public string[] MediaUrl { get; set; }
        public string CorrectAnswer { get; set; }

        #endregion

        #region Constructors

        public Question()
            : this(-1, QuestionType.Text, "") { }

        public Question(long id, QuestionType type, string answer)
            : base(id) {
            Type = type;
            CorrectAnswer = answer;
        }

        public Question(Question other)
            : this(other.Id,
                   other.Type,
                   other.CorrectAnswer) { }

        #endregion

        #region Equality Members

        internal override bool _Equals(Question other) {
            return Equals(Type, other.Type) &&
                   string.Equals(CorrectAnswer, other.CorrectAnswer);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Type.GetHashCode());
                hashCode = (hashCode * 397) ^ (CorrectAnswer != null ? CorrectAnswer.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }

    public enum QuestionType {
        Text,
        Sound,
        Image
    }
}
