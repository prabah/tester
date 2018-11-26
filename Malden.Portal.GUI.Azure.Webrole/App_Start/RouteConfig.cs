using System.Web.Mvc;
using System.Web.Routing;

namespace Malden.Portal.GUI.Azure.Webrole
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("search");
            routes.IgnoreRoute("manager/html");
            routes.IgnoreRoute("{controller}/search");
            routes.IgnoreRoute("{controller}/manager/html");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute("Error", "{*url}",
                new { controller = "Error", action = "http404" }
            );
            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }
    }
}