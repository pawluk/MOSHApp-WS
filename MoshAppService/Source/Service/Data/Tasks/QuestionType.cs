// Project: MoshAppService
// Filename: QuestionType.cs
// 
// Author: Jason Recillo

using System;

namespace MoshAppService.Service.Data.Tasks {
    // Flags attribute is so that ServiceStack.Text will serialize this enum as a number rather than text
    [Flags]
    public enum QuestionType {
        Normal = 1,
        MultipleChoice = 2
    }
}
