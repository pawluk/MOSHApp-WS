// Project: MoshAppService
// Filename: NHibernateHelper.cs
// 
// Author: Jason Recillo

using System;
using System.Reflection;
using System.Web;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;

namespace MoshAppService.Service.Database {
    public class NHibernateHelper {
        private const string CurrentSessionKey = "nhibernate.current_session";
        private static readonly ISessionFactory SessionFactory;

        static NHibernateHelper() {
            var mapper = new ModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());

            mapper.BeforeMapClass += (mi, t, map) => map.Table(t.Name.ToLowerInvariant());
            mapper.BeforeMapJoinedSubclass += (mi, t, map) => map.Table(t.Name.ToLowerInvariant());
            mapper.BeforeMapUnionSubclass += (mi, t, map) => map.Table(t.Name.ToLowerInvariant());

            var configuration = new Configuration();
            configuration.DataBaseIntegration(c => {
                c.Dialect<MySQL5Dialect>();
                c.ConnectionString = "Server=localhost;Database=c9mosh;User ID=c9mosh;Password=moshgbc;";
                c.LogSqlInConsole = true;
            });

            configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
            SessionFactory = configuration.BuildSessionFactory();
        }

        public static ISession GetCurrentSession() {
            var context = HttpContext.Current;
            var currentSession = context.Items[CurrentSessionKey] as ISession;

            if (currentSession == null) {
                currentSession = SessionFactory.OpenSession();
                context.Items[CurrentSessionKey] = currentSession;
            }

            return currentSession;
        }

        public static void CloseSession() {
            var context = HttpContext.Current;
            var currentSession = context.Items[CurrentSessionKey] as ISession;

            if (currentSession == null) {
                // no current session
                return;
            }

            currentSession.Close();
            context.Items.Remove(CurrentSessionKey);
        }

        public static void CloseSessionFactory() {
            if (SessionFactory != null) SessionFactory.Close();
        }
    }
}
