// Project: MoshAppService
// Filename: Question.cs
// 
// Author: Jason Recillo

using System;

namespace MoshAppService.Service.Data.Tasks {
    public class Question : Entity<Question> {
        public QuestionType Type { get; set; }
        public string QuestionText { get; set; }

        protected override bool _Equals(Question other) {
            throw new NotImplementedException();
        }
    }
}
