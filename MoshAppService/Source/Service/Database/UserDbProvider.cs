// Project: MoshAppService
// Filename: UserDbProvider.cs
// 
// Author: Jason Recillo

using System;
using System.Collections.Generic;

using MoshAppService.Service.Data;

using ServiceStack.OrmLite;

namespace MoshAppService.Service.Database {
    public class UserDbProvider : BaseDbProvider<User> {
        #region Lazy-Initialized Singleton

        private static readonly Lazy<UserDbProvider> _instance = new Lazy<UserDbProvider>(() => new UserDbProvider());
        public static UserDbProvider Instance { get { return _instance.Value; } }
        internal UserDbProvider() { }

        #endregion

        #region Temporary in-memory "Database" (소녀시대) :D

        internal static readonly Dictionary<long, User> Users = new Dictionary<long, User> {
            {
                0, new User {
                    Id = 0,
                    FirstName = "Taeyeon",
                    LastName = "Kim",
                    Email = "example01@example.com",
                    Phone = "6470010101",
                    StudentNumber = "100123456"
                }
            }, {
                1, new User {
                    Id = 1,
                    FirstName = "Tiffany",
                    LastName = "Hwang",
                    Email = "example02@example.com",
                    Phone = "6470020202",
                    StudentNumber = "100987654"
                }
            }, {
                2, new User {
                    Id = 2,
                    FirstName = "Soonkyu",
                    LastName = "Lee",
                    Email = "example03@example.com",
                    Phone = "4160030303",
                    StudentNumber = "100159753"
                }
            }, {
                3, new User {
                    Id = 3,
                    FirstName = "Sooyeon",
                    LastName = "Jung",
                    Email = "example04@example.com",
                    Phone = "6470040404",
                    StudentNumber = "100456852"
                }
            }, {
                4, new User {
                    Id = 4,
                    FirstName = "Joohyun",
                    LastName = "Seo",
                    Email = "example05@example.com",
                    Phone = "6470050505",
                    StudentNumber = "100147896"
                }
            }, {
                5, new User {
                    Id = 5,
                    FirstName = "Sooyoung",
                    LastName = "Choi",
                    Email = "example06@example.com",
                    Phone = "6470060606",
                    StudentNumber = "100456963"
                }
            }, {
                6, new User {
                    Id = 6,
                    FirstName = "Yuri",
                    LastName = "Kwon",
                    Email = "example07@example.com",
                    Phone = "6470070707",
                    StudentNumber = "100987123"
                }
            }, {
                7, new User {
                    Id = 7,
                    FirstName = "Hyoyeon",
                    LastName = "Kim",
                    Email = "example08@example.com",
                    Phone = "6470090909",
                    StudentNumber = "100741963"
                }
            }, {
                8, new User {
                    Id = 8,
                    FirstName = "Yoona",
                    LastName = "Im",
                    Email = "example09@example.com",
                    Phone = "6470101010",
                    StudentNumber = "100793158"
                }
            }
        };

        #endregion

        //        public User this[long id] {{}
        //            try {
        //                return Users[id];
        //            } catch (InvalidOperationException) {
        //                return null;
        //            }
        //        }
        protected override void InitializeDb() {
            using (var db = DbFactory.OpenDbConnection()) {
                db.DropTable<User>();
                db.CreateTable<User>();
                foreach (var user in Users.Values) db.Insert(user);
            }
        }

        public override User this[long id] {
            get {
                using (var db = DbFactory.OpenDbConnection())
                    return db.GetById<User>(id);
            }
        }
    }
}
