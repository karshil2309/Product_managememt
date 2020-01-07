using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Project1
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
/*
            routes.MapRoute(
                name: "Serial Route",
                url: "serial/{letterCase}",
                defaults: new {Controller ="User", action="Serial", lettercase="upper" }
                );
*/
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Admin", action = "login", id = UrlParameter.Optional }
            );

        }
    }
}
