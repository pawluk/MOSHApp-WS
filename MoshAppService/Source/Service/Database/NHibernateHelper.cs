// Project: MoshAppService
// Filename: NHibernateHelper.cs
// 
// Author: Jason Recillo

using System;
using System.Configuration;
using System.Reflection;
using System.Web;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;

using ServiceStack.Logging;

using Configuration = NHibernate.Cfg.Configuration;

namespace MoshAppService.Service.Database {
    public static class NHibernateHelper {
        private const string CurrentSessionKey = "nhibernate.current_session";
        private static readonly ISessionFactory SessionFactory;

        static NHibernateHelper() {
            LogManager.GetLogger(typeof(NHibernateHelper)).Info("Instantiating NHibernate...");

            var mapper = new ModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());

            mapper.BeforeMapClass += (mi, t, map) => map.Table(t.Name.ToLowerInvariant());
            mapper.BeforeMapJoinedSubclass += (mi, t, map) => map.Table(t.Name.ToLowerInvariant());
            mapper.BeforeMapUnionSubclass += (mi, t, map) => map.Table(t.Name.ToLowerInvariant());

            var configuration = new Configuration();
            configuration.DataBaseIntegration(c => {
                c.Dialect<MySQL5Dialect>();
                c.ConnectionString = ConfigurationManager.ConnectionStrings["MoshConnection"].ConnectionString;
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

        public static ISession GetCurrentSessionAndStartTransaction(out ITransaction tx) {
            var session = GetCurrentSession();
            tx = session.BeginTransaction();
            return session;
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

        public static void EndTransactionAndCloseSession(ITransaction transaction) {
            transaction.Commit();
            CloseSession();
        }

        public static void CloseSessionFactory() {
            if (SessionFactory != null) SessionFactory.Close();
        }
    }
}
