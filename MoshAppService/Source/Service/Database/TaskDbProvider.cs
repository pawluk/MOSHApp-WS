// Project: MoshAppService
// Filename: TaskDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;
using System.Data;

using MoshAppService.Service.Data.Tasks;

using MySql.Data.MySqlClient;

namespace MoshAppService.Service.Database {
    public class TaskDbProvider : BaseDbProvider<Task> {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<TaskDbProvider> _instance = new Lazy<TaskDbProvider>(() => new TaskDbProvider());
        private TaskDbProvider() { }
        public static TaskDbProvider Instance { get { return _instance.Value; } }

        #endregion

        #region Temporary in-memory "Database"

        internal static readonly Dictionary<long, Task> Tasks = new Dictionary<long, Task> {
            {
                0, new Task {
                    Id = 0,
                    Campus = new Campus {
                        Id = 2,
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
                        Id = 2,
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

        public override Task this[long id] {
            get {
                try {
                    return Tasks[id];
                } catch (InvalidOperationException) {
                    return null;
                }
            }
        }

        protected override Task BuildObject(MySqlDataReader reader) {
            throw new NotImplementedException();
        }
    }
}
