using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;

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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

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

        [System.Web.Http.HttpPost]
        public string saveContentPropertyValue([FromBody] string paramValues) // int contentId, string propertyAlias, [FromBody] string propertyValue)
        {
            string result = "Unexpected error.";

            // Initializations
            dynamic paramObject = null;

            // Check whether parameters have a value
            if (paramValues == null)
            {
                result = "Parameters are null.";
                return result;
            }

            // Parse parameters
            paramObject = JObject.Parse(paramValues);
            if (paramObject == null)
            {
                result = "Parameters are incorrect.";
                return result;
            }

            // Get the values of the parameters
            int contentId = int.MinValue;
            string propertyAlias = string.Empty;
            string propertyValue = string.Empty;
            try
            {
                contentId = paramObject.contentId;
                propertyAlias = paramObject.propertyAlias;
                propertyValue = paramObject.propertyValue;
            }
            catch (Exception ex)
            {
                result = string.Format("Error getting the parameters value: {0}", ex.Message);
                return result;
            }

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
                    result = string.Empty;
                }
            }
            return result;
        }

    }
}




