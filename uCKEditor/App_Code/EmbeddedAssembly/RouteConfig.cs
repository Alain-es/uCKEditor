using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Umbraco.Core.Logging;

namespace uCKEditor.EmbeddedAssembly
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            const string pluginBasePath = "App_Plugins/uCKEditor";
            string url = string.Empty;

            RouteTable.Routes.MapRoute(
                name: "GetResourcePath0",
                url: pluginBasePath + "/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath0"
                }
            );

            RouteTable.Routes.MapRoute(
                name: "GetResourcePath1",
                url: pluginBasePath + "/{directory1}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath1"
                }
            );

            RouteTable.Routes.MapRoute(
                name: "GetResourcePath2",
                url: pluginBasePath + "/{directory1}/{directory2}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath2"
                }
            );

            RouteTable.Routes.MapRoute(
                name: "GetResourcePath3",
                url: pluginBasePath + "/{directory1}/{directory2}/{directory3}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath3"
                }
            );

            RouteTable.Routes.MapRoute(
                name: "GetResourcePath4",
                url: pluginBasePath + "/{directory1}/{directory2}/{directory3}/{directory4}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath4"
                }
            );

            RouteTable.Routes.MapRoute(
                name: "GetResourcePath5",
                url: pluginBasePath + "/{directory1}/{directory2}/{directory3}/{directory4}/{directory5}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath5"
                }
            );

        }
    }
}