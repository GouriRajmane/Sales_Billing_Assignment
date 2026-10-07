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

            // Custom route for SalesInvoice default location
            routes.MapRoute(
                name: "SalesInvoiceDefault",
                url: "SalesInvoice",
                defaults: new { controller = "SalesInvoice", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "SalesInvoice", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
