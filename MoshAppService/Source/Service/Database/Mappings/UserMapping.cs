// Project: MoshAppService
// Filename: UserMapping.cs
// 
// Author: Jason Recillo

using System;

using JetBrains.Annotations;

using MoshAppService.Service.Data;

using NHibernate.Mapping.ByCode.Conformist;

namespace MoshAppService.Service.Database.Mappings {
    [UsedImplicitly]
    public class UserMapping : ClassMapping<User> {
        public UserMapping() {
            Table("users");
            EntityName("User");
            Lazy(false);
            Id(x => x.Id, map => map.Column("u_id"));
            Property(x => x.Nickname, map => map.Column("u_nicknme"));
            Property(x => x.FirstName, map => map.Column("u_fname"));
            Property(x => x.LastName, map => map.Column("u_lastname"));
            Property(x => x.Email, map => map.Column("u_email"));
            Property(x => x.Phone, map => map.Column("u_phone"));
            Property(x => x.StudentNumber, map => map.Column("s_num"));
        }
    }
}
