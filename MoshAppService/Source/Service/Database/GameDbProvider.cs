// Project: MoshAppService
// Filename: GameDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;

using MoshAppService.Service.Data;
using MoshAppService.Service.Data.Tasks;

namespace MoshAppService.Service.Database {
    public class GameDbProvider : BaseDbProvider {
        #region Temporary in-memory "Database"

        internal static readonly Dictionary<long, Game> Games = new Dictionary<long, Game> {
            {
                0, new Game {
                    Id = 0,
                    Team = TeamDbProvider.GetTeam(0),
                    Tasks = new HashSet<Task> {
                        TaskDbProvider.GetTask(0),
                        TaskDbProvider.GetTask(1)
                    },
                    Start = new DateTime(2012, 12, 21, 10, 0, 0),
                    Finish = new DateTime(2012, 12, 22, 10, 0, 0)
                }
            }, {
                1, new Game {
                    Id = 1,
                    Team = TeamDbProvider.GetTeam(1),
                    Tasks = new HashSet<Task> {
                        TaskDbProvider.GetTask(1),
                        TaskDbProvider.GetTask(0)
                    },
                    Start = new DateTime(2012, 12, 21, 10, 0, 0),
                    Finish = new DateTime(2012, 12, 22, 10, 0, 0)
                }
            }
        };

        #endregion

        public static Game GetGame(long id) {
            try {
                return Games[id];
            } catch (InvalidOperationException) {
                return null;
            }
        }
    }
}
