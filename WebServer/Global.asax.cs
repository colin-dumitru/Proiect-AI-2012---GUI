using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebServer {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            /*rutarea catre incarcarea de fisier*/
            routes.MapRoute(
                "Upload", // Route name
                "Up", // URL with parameters
                new { controller = "Load", action = "Upload" }
            );

            /*rutarea catre incarcarea unui document deja existent*/
            routes.MapRoute(
                "Load", // Route name
                "Load/{id}", // URL with parameters
                new { controller = "Load", action = "Load", id = UrlParameter.Optional}
            );            

            /*rutarea catre controllerul principal*/
            routes.MapRoute(
                "Home", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            /*rutarea catre controllerul de cereri XML*/
            routes.MapRoute(
                "Get", // Route name
                "{controller}/{action}/{id}/{question}", // URL with parameters
                new { controller = "Get", action = "Summary", id = UrlParameter.Optional, question = UrlParameter.Optional } 
            );

        }

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
    }
}