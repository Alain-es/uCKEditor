using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

using umbraco;
using Umbraco.Core;
using Umbraco.Core.Dynamics;
using Umbraco.Core.Models;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using Umbraco.Core.Persistence.DatabaseModelDefinitions;
using umbraco.BusinessLogic.Actions;
using umbraco.BusinessLogic;
using Umbraco.Web;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using Umbraco.Web.Models.ContentEditing;

using AutoMapper;

namespace uCKEditor.Controllers.Api
{
    [PluginController("uCKEditor")]
    [IsBackOffice]
    public class uCKEditorApiController : UmbracoAuthorizedJsonController
    {
        //[HttpGet]
        //public string getContentPropertyValue(int contentId, string propertyAlias)
        //{
        //    string result = string.Empty;
        //    var content = ApplicationContext.Current.Services.ContentService.GetById(contentId);
        //    if (content != null)
        //    {
        //        var property = content.Properties.Where(p => p.Alias == propertyAlias).FirstOrDefault();
        //        if (property != null)
        //        {
        //            result = property.Value.ToString();
        //        }
        //    }
        //    return result;
        //}

        [HttpGet]
        public bool getSavedContentPropertyValue(int contentId, string propertyAlias, string propertyValue)
        {
            var result = false;
            var content = ApplicationContext.Current.Services.ContentService.GetById(contentId);
            if (content != null)
            {
                var property = content.Properties.Where(p => p.Alias == propertyAlias).FirstOrDefault();
                if (property != null)
                {
                    property.Value = propertyValue;
                    if(content.Published)
                        ApplicationContext.Services.ContentService.SaveAndPublishWithStatus(content);
                    else
                        ApplicationContext.Services.ContentService.Save(content);
                    result = true;
                }
            }
            return result;
        }

    }
}




