// Project: MoshAppService
// Filename: TaskDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;

using MoshAppService.Service.Data.Tasks;

namespace MoshAppService.Service.Database {
    public class TaskDbProvider : BaseDbProvider<Task> {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<TaskDbProvider> _instance = new Lazy<TaskDbProvider>(() => new TaskDbProvider());
        public static TaskDbProvider Instance { get { return _instance.Value; } }
        private TaskDbProvider() { }

        #endregion
        #region Temporary in-memory "Database"

        internal static readonly Dictionary<long, Task> Tasks = new Dictionary<long, Task> {
            {
                0, new Task {
                    Id = 0,
                    Campus = new Campus {
                        Id = 0,
                        Name = "Casa Loma",
                        Latitude = 43.676187,
                        Longitude = -79.410076
                    },
                    Direction = new Direction {
                        Id = 0,
                        Text = "What is the thing here?",
                        Latitude = 43.676187,
                        Longitude = -79.410076
                    },
                    Question = new Question {
                        Id = 0,
                        CorrectAnswer = "A thing",
                        Type = QuestionType.Text
                    },
                    Previous = null
                }
            }, {
                1, new Task {
                    Id = 1,
                    Campus = new Campus {
                        Id = 0,
                        Name = "Casa Loma",
                        Latitude = 43.676187,
                        Longitude = -79.410076
                    },
                    Direction = new Direction {
                        Id = 1,
                        Text = "Go to this place. What do you see?",
                        Latitude = 43.676187,
                        Longitude = -79.410076
                    },
                    Question = new Question {
                        Id = 1,
                        CorrectAnswer = "A thing",
                        Type = QuestionType.Text
                    },
                    Previous = null
                }
            }
        };

        #endregion

        protected override void InitializeDb() {
            throw new NotImplementedException();
        }

        public override Task this[long id] { get { throw new NotImplementedException(); } }

        public static Task GetTask(long id) {
            try {
                return Tasks[id];
            } catch (InvalidOperationException) {
                return null;
            }
        }
    }
}
