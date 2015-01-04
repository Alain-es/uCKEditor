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
                name: "uCKEditor.GetResourcePath0",
                url: pluginBasePath + "/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath0"
                },
                namespaces: new[] { "uCKEditor.EmbeddedAssembly" }
            );

            RouteTable.Routes.MapRoute(
                name: "uCKEditor.GetResourcePath1",
                url: pluginBasePath + "/{directory1}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath1"
                },
                namespaces: new[] { "uCKEditor.EmbeddedAssembly" }
            );

            RouteTable.Routes.MapRoute(
                name: "uCKEditor.GetResourcePath2",
                url: pluginBasePath + "/{directory1}/{directory2}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath2"
                },
                namespaces: new[] { "uCKEditor.EmbeddedAssembly" }
            );

            RouteTable.Routes.MapRoute(
                name: "uCKEditor.GetResourcePath3",
                url: pluginBasePath + "/{directory1}/{directory2}/{directory3}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath3"
                },
                namespaces: new[] { "uCKEditor.EmbeddedAssembly" }
            );

            RouteTable.Routes.MapRoute(
                name: "uCKEditor.GetResourcePath4",
                url: pluginBasePath + "/{directory1}/{directory2}/{directory3}/{directory4}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath4"
                },
                namespaces: new[] { "uCKEditor.EmbeddedAssembly" }
            );

            RouteTable.Routes.MapRoute(
                name: "uCKEditor.GetResourcePath5",
                url: pluginBasePath + "/{directory1}/{directory2}/{directory3}/{directory4}/{directory5}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath5"
                },
                namespaces: new[] { "uCKEditor.EmbeddedAssembly" }
            );

            RouteTable.Routes.MapRoute(
                name: "uCKEditor.GetResourcePath6",
                url: pluginBasePath + "/{directory1}/{directory2}/{directory3}/{directory4}/{directory5}/{directory6}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath6"
                },
                namespaces: new[] { "uCKEditor.EmbeddedAssembly" }
            );

            RouteTable.Routes.MapRoute(
                name: "uCKEditor.GetResourcePath7",
                url: pluginBasePath + "{directory1}/{directory2}/{directory3}/{directory4}/{directory5}/{directory6}/{directory7}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath7"
                },
                namespaces: new[] { "uCKEditor.EmbeddedAssembly" }
            );

        }
    }
}