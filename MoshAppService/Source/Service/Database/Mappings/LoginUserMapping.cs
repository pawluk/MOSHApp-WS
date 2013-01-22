// Project: MoshAppService
// Filename: LoginUserMapping.cs
// 
// Author: Jason Recillo

using System;

using JetBrains.Annotations;

using MoshAppService.Service.Data;

using NHibernate.Mapping.ByCode.Conformist;

namespace MoshAppService.Service.Database.Mappings {
    [UsedImplicitly]
    public class LoginUserMapping : ClassMapping<LoginUser> {
        public LoginUserMapping() {
            Table("login");
            EntityName("LoginUser");
            Lazy(false);
            Property(x => x.LoginName, map => map.Column("login_name"));
            Property(x => x.Password, map => map.Column("login_pass"));
            RegisterOneToOneMapping<User>(x => x.ForeignKey("login_ibfk_1"));
            Id(x => x.Id, map => map.Column("u_id"));
        }
    }
}
