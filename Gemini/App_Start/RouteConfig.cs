using System;
using Gemini.Models;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gemini
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            try
            {
                routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

                #region Add portal admin in router
                var db = new GeminiEntities();

                routes.MapRoute(
                    name: "admin",
                    url: "it4788" + "/{controller}/{action}",
                    defaults: new { Portal = "admin", controller = "Default", action = "Index" }
                );
                #endregion
            }
            catch (Exception ex)
            {

            }
        }
    }
}