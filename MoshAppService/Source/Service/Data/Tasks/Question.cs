// Project: MoshAppService
// Filename: Question.cs
// 
// Author: Jason Recillo

using System;

using MoshAppService.Utils;

namespace MoshAppService.Service.Data.Tasks {
    public class Question : Entity<Question> {
        private string _questionText;

        public QuestionType Type { get; set; }
        public string QuestionText { get { return _questionText; } set { _questionText = value.NormalizeSpaces(); } }

        protected override bool _Equals(Question other) {
            return Type.Equals(other.Type) &&
                   QuestionText.Equals(other.QuestionText);
        }
    }
}
