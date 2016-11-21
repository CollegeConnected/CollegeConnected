﻿using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using CollegeConnected.Imports;
using System.Web.Providers;

namespace CollegeConnected
{
    public class MvcApplication : HttpApplication
    {
        public static CollegeConnectedImporterBase CurrentImport = null;
        public static Task ImportTask = null;

        public static MvcApplication CollegeConnectedApplication { get; set; }

        protected void Application_Start()
        {
            CollegeConnectedApplication = this;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CurrentImport = new CollegeConnectedImporterBase();
        }

    }
}