﻿// Project: MoshAppService
// Filename: Global.asax.cs
// 
// Date: 23/12/2012
// Time: 9:29 PM
// 
// Author: Jason Recillo

using System;
using System.Web;

using MoshAppService.Service;

using ServiceStack.Logging;
using ServiceStack.Logging.NLogger;
using ServiceStack.MiniProfiler;

namespace MoshAppService {
    public class Global : HttpApplication {
        protected void Application_Start(object sender, EventArgs e) {
            LogManager.LogFactory = new NLogFactory();
            LogManager.GetLogger(typeof(Global)).Info("Application start!");
            new MoshAppServiceHost().Init();
        }

        protected void Session_Start(object sender, EventArgs e) { }

        protected void Application_BeginRequest(object sender, EventArgs e) {
            if (Request.IsLocal) Profiler.Start(ProfileLevel.Verbose);
        }

        protected void Application_EndRequest(object sender, EventArgs e) {
            if (Request.IsLocal) Profiler.Stop();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e) { }

        protected void Application_Error(object sender, EventArgs e) { }

        protected void Session_End(object sender, EventArgs e) { }

        protected void Application_End(object sender, EventArgs e) { }
    }
}
