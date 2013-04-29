// Project: MoshAppService
// Filename: Answer.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MoshAppService.Service.Data.Tasks {
    // TODO: Question. Does this object ever leave the web service, or is it just used internally to verify correct answers and the like?
    public class Answer : Entity<Answer> {
        public Question Question { get; set; }
        public string AnswerText { get; set; }

        protected override bool _Equals(Answer other) {
            return Question.Equals(other.Question) &&
                   AnswerText.Equals(other.AnswerText);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Question != null ? Question.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AnswerText != null ? AnswerText.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
