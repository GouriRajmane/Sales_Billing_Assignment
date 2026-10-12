using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Sales_Billing_System
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // Global flag to track if this is the very first request
        private static bool _isFirstRequest = true;
        private static readonly object _lockObject = new object();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Check if it's the first request in a thread-safe manner
            if (_isFirstRequest)
            {
                lock (_lockObject)
                {
                    if (_isFirstRequest)
                    {
                        _isFirstRequest = false;

                        // Define your default landing page URL structure here
                        // For example: "/Home/Index" or a specific route
                        string defaultUrl = UrlHelper.GenerateContentUrl("~/SalesInvoice/Index", new HttpContextWrapper(HttpContext.Current));

                        // If you want a specific controller/action explicitly:
                        // string defaultUrl = "/Home/Index"; 

                        // Force redirect the very first request to the default page
                        Response.Redirect(defaultUrl, true);
                    }
                }
            }
        }
    }
}
