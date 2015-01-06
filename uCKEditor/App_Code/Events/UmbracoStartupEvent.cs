using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using umbraco.cms.presentation;
using Umbraco.Core;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Logging;

using uCKEditor.EmbeddedAssembly;

namespace uCKEditor.Events
{
    public class UmbracoStartupEvent : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            //LogHelper.Info(typeof(UmbracoStartupEvent), string.Format("Startup event ..."));

            // Add the 'uCKEditor' section to all users 
            // TODO: Replaced the code below with the method Services.UserServiceAddSectionToAllUsers() (PR: https://github.com/umbraco/Umbraco-CMS/pull/614/files)
            int users = 0;
            var allUsers = ApplicationContext.Current.Services.UserService.GetAll(0, 1000, out users);
            foreach (var user in allUsers.Where(u => !u.AllowedSections.Contains("uckeditor")))
            {
                //now add the section for each user and commit
                user.AddAllowedSection("uckeditor");
                ApplicationContext.Current.Services.UserService.Save(user);
            }

            // Register routes for embedded files
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}