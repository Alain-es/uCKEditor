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

using uCKEditor.Helpers;

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
            bool errorAddedToLog = false;

            try
            {

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
                result = "Error getting the parameters value";
            int contentId = int.MinValue;
            string propertyAlias = string.Empty;
            string propertyValue = string.Empty;
                contentId = paramObject.contentId;
                propertyAlias = paramObject.propertyAlias;
                propertyValue = paramObject.propertyValue;

                result = "Error saving property value.";
            var content = ApplicationContext.Current.Services.ContentService.GetById(contentId);
            if (content != null)
            {
                // Check whether the property alias contains dots. If the property alias contains any dot then it means that the property is inside an archetype property (since dots are forbidden in Umbraco content property aliases)
                if (propertyAlias.Contains("."))
                {
                    content = ArchetypeHelper.SetArchetypePropertyValue(content, propertyAlias, propertyValue);
                }
                else
                {
                    var property = content.Properties.Where(p => p.Alias == propertyAlias).FirstOrDefault();
                    if (property != null)
                    {
                        property.Value = propertyValue;
                    }
                }
                if (content.Published)
                    ApplicationContext.Services.ContentService.SaveAndPublishWithStatus(content);
                else
                    ApplicationContext.Services.ContentService.Save(content);
                result = string.Empty;
            }
            }

            catch (Exception ex)
            {
                LogHelper.Error<uCKEditorApiController>(result, ex);
                errorAddedToLog = true;
                result = string.Format("{1}: {0}", result, ex.Message);
                return result;
            }

            finally
            {
                // If there is an error add it to the log file
                if (!errorAddedToLog && !string.IsNullOrWhiteSpace(result))
                {
                    LogHelper.Error<uCKEditorApiController>(result, null);
                }
            }

            return result;
        }

        [System.Web.Http.HttpGet]
        public string getUniqueId(string uniqueIdType)
        {
            string result = string.Empty;
            switch (uniqueIdType.Trim().ToLower())
            {
                case "guidnodashes":
                    result = Guid.NewGuid().ToString().Replace("-", string.Empty);
                    break;
                case "hashcodefromguid":
                    result = Guid.NewGuid().ToString().GetHashCode().ToString().Replace("-", "1");
                    break;
                case "datetime":
                    var random = new Random(DateTime.Now.Millisecond);
                    System.Threading.Thread.Sleep(random.Next(10));
                    result = string.Format("{0:yyyyMMddHHmmssfffffff}", DateTime.Now);
                    break;
                default:
                    result = Guid.NewGuid().ToString();
                    break;
            }
            return result;
        }

    }
}




