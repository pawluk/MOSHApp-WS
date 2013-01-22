// Project: MoshAppService
// Filename: GameDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;
using System.Linq;

using MoshAppService.Service.Data;
using MoshAppService.Service.Data.Tasks;

namespace MoshAppService.Service.Database {
    public class GameDbProvider : BaseDbProvider<Game> {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<GameDbProvider> _instance = new Lazy<GameDbProvider>(() => new GameDbProvider());
        private GameDbProvider() { }
        public static GameDbProvider Instance { get { return _instance.Value; } }

        #endregion

        #region Temporary in-memory "Database"

        internal static readonly Dictionary<long, Game> Games = new Dictionary<long, Game> {
            {
                0, new Game {
                    Id = 0,
                    Team = TeamDbProvider.Instance[0],
                    Tasks = new HashSet<Task> {
                        TaskDbProvider.Instance[0],
                        TaskDbProvider.Instance[1]
                    },
                    Start = new DateTime(2012, 12, 21, 10, 0, 0, DateTimeKind.Local),
                    Finish = new DateTime(2012, 12, 22, 12, 0, 0, DateTimeKind.Local)
                }
            }, {
                1, new Game {
                    Id = 1,
                    Team = TeamDbProvider.Instance[1],
                    Tasks = new HashSet<Task> {
                        TaskDbProvider.Instance[1],
                        TaskDbProvider.Instance[0]
                    },
                    Start = new DateTime(2012, 12, 21, 10, 0, 0, DateTimeKind.Local),
                    Finish = new DateTime(2012, 12, 22, 10, 0, 0, DateTimeKind.Local)
                }
            }
        };

        #endregion

        public override Game this[long id] {
            get {
                try {
                    return Games[id];
                } catch (InvalidOperationException) {
                    return null;
                }
            }
        }

        public Game this[Team team] {
            get {
                //
                return Games.ToList().Find(game => { return game.Value.Team.Equals(team); }).Value;
            }
        }
    }
}
