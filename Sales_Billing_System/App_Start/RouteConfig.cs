using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sales_Billing_System
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Catch-all route to map any URL to the default controller and action
            //routes.MapRoute(
            //    name: "CatchAllDefault",
            //    url: "{*catchall}",
            //    defaults: new { controller = "SalesInvoice", action = "Index", id = UrlParameter.Optional }

            //);

            // Custom route for SalesInvoice default location
            routes.MapRoute(
                name: "SalesInvoiceDefault",
                url: "SalesInvoice",
                defaults: new { controller = "SalesInvoice", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }

            );
        }
    }
}
