// Project: MoshAppService
// Filename: QuestionType.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MoshAppService.Service.Data.Tasks {
    // Flags attribute is so that ServiceStack.Text will serialize this enum as a number rather than text
    [Flags]
    public enum QuestionType {
        Normal = 1,
        MultipleChoice = 2
    }
}
