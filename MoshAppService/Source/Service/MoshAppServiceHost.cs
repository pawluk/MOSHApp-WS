// Project: MoshAppService
// Filename: MoshAppServiceHost.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Funq;

using JetBrains.Annotations;

using MoshAppService.Service.Security;
using MoshAppService.Utils;

using ServiceStack.CacheAccess.Providers;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.Cors;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Extensions;

namespace MoshAppService.Service {
    [PublicAPI]
    public class MoshAppServiceHost : AppHostBase {
        private static readonly ILog Log;

        static MoshAppServiceHost() {
            Log = LogManager.GetLogger(typeof(MoshAppServiceHost));
        }

        public MoshAppServiceHost()
            : base("Mobile Orientation Scavenger Hunt - Web Service", typeof(MoshAppServiceHost).Assembly) { }

        /// <summary>
        /// Configure the given container with the registrations provided by the funqlet.
        /// </summary>
        /// <param name="container">Container to register.</param>
        public override void Configure(Container container) {
            Log.Info("Starting service...");

            // Set up base configuration
            const Feature disableFeatures = Feature.Soap | Feature.Xml | Feature.Csv | Feature.Jsv;
            SetConfig(new EndpointHostConfig {
                EnableFeatures = Feature.All.Remove(disableFeatures),
#if DEBUG
                DebugMode = true,
#endif
                DefaultContentType = ContentType.Json
            });

            // Set JSON services to return "camelCase" rather than "CamelCase"
            JsConfig.EmitCamelCaseNames = true;

            // Set dates to serialize to ISO-8601 dates
            JsConfig.DateHandler = JsonDateHandler.ISO8601;

            // Don't serialize type information
            JsConfig.ExcludeTypeInfo = true;

            // Set up authentication service
            Plugins.Add(new AuthFeature(() => new AuthUserSession(), new IAuthProvider[] { new MoshAppAuthProvider() }) {
                ServiceRoutes = new Dictionary<Type, string[]> {
                    { typeof(AuthService), new[] { "/authenticate" } },
                },
                HtmlRedirect = null,
                IncludeAssignRoleServices = false,
            });

            // Enable CORS (Cross-Origin Resource Sharing)
            // http://stackoverflow.com/questions/8211930/servicestack-rest-api-and-cors
            Plugins.Add(new CorsFeature());
            PreRequestFilters.Add((req, res) => {
                if (req.HttpMethod == "OPTIONS") res.EndServiceStackRequest();
            });

            container.Register((Global.Cache = new MemoryCacheClient()));

            Log.Debug(Config.Metadata.Dump());
            Log.Info("Service started!");
            Log.Debug("Debug mode is {0}!".F(Config.DebugMode ? "ON" : "OFF"));
        }
    }
}
