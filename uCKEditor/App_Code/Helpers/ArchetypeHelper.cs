using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

using System.Web.Hosting;
using System.IO;

using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Data.OleDb;

using Archetype.Models;

namespace uCKEditor.Helpers
{

    public class ArchetypeHelper
    {

        private class ArchetypePropertyInfo
        {
            public string FieldsetName { get; set; }
            public string IdPropertyName { get; set; }
            public string IdPropertyValue { get; set; }
        }

        public static object GetArchetypePropertyValue(int contentId, string propertyAlias)
        {
            return GetArchetypePropertyValue(UmbracoContext.Current.Application.Services.ContentService.GetById(contentId), propertyAlias);
        }

        public static object GetArchetypePropertyValue(IContent content, string propertyAlias)
        {
            // TODO: Support deeper levels in the archetype hierarchy. For the moment only properties at the first level are permitted.
            object result = null;
            if (content != null)
            {
                var fieldsetInfo = ExtractPropertyInfo(propertyAlias);
                if (fieldsetInfo.Count > 0)
                {
                    var property = content.Properties.Where(p => p.Alias == fieldsetInfo[0].FieldsetName).FirstOrDefault();
                    if (property != null)
                    {
                        // Get archetype model
                        var archetypeModel = JsonConvert.DeserializeObject<ArchetypeModel>(property.Value.ToString());
                        if (archetypeModel != null)
                        {
                            foreach (var fieldset in archetypeModel.Fieldsets)
                            {
                                if (fieldset.Properties.Where(p => p.Alias == fieldsetInfo[0].IdPropertyName && p.Value.ToString() == fieldsetInfo[0].IdPropertyValue).Any())
                                {
                                    result = fieldset.GetValue<string>(fieldsetInfo[1].FieldsetName);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static int GetArchetypePropertyDatatypeId(IContent content, string propertyAlias)
        {
            // TODO: Support deeper levels in the archetype hierarchy. For the moment only properties at the first level are permitted.
            int result = int.MinValue;
            if (content != null)
            {
                var fieldsetInfo = ExtractPropertyInfo(propertyAlias);
                if (fieldsetInfo.Count > 0)
                {
                    var contentType = UmbracoContext.Current.Application.Services.ContentTypeService.GetContentType(content.ContentTypeId);
                    if (contentType != null)
                    {
                        var propertyTypes = contentType.PropertyTypes.Where(p => p.Alias == fieldsetInfo[0].FieldsetName);
                        if (propertyTypes.Count() > 0)
                        {
                            var propertyEditorAlias = propertyTypes.FirstOrDefault().PropertyEditorAlias;
                            var dataTypeDefinition = UmbracoContext.Current.Application.Services.DataTypeService.GetDataTypeDefinitionByPropertyEditorAlias(propertyEditorAlias);
                            if (dataTypeDefinition.Count() > 0)
                            {
                                var prevalues = UmbracoContext.Current.Application.Services.DataTypeService.GetPreValuesByDataTypeId(dataTypeDefinition.FirstOrDefault().Id);
                                if (prevalues.Count() > 0)
                                {
                                    var archetypeDefinition = JsonConvert.DeserializeObject<ArchetypePreValue>(prevalues.FirstOrDefault());
                                    if (archetypeDefinition != null)
                                    {
                                        var archetypePropertyDefinition = archetypeDefinition.Fieldsets.First().Properties.First(p => p.Alias == fieldsetInfo[1].FieldsetName);
                                        if (archetypePropertyDefinition != null)
                                        {
                                            var archetypePropertyDataType = UmbracoContext.Current.Application.Services.DataTypeService.GetDataTypeDefinitionById(archetypePropertyDefinition.DataTypeGuid);
                                            if (archetypePropertyDataType != null)
                                            {
                                                result = archetypePropertyDataType.Id;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }


        public static IContent SetArchetypePropertyValue(IContent content, string propertyAlias, object value)
        {
            // TODO: Support deeper levels in the archetype hierarchy. For the moment only properties at the first level are permitted.
            IContent result = content;
            if (content != null)
            {
                var fieldsetInfo = ExtractPropertyInfo(propertyAlias);
                if (fieldsetInfo.Count > 0)
                {
                    var property = content.Properties.Where(p => p.Alias == fieldsetInfo[0].FieldsetName).FirstOrDefault();
                    if (property != null && property.Value != null)
                    {
                        // Get archetype model
                        var archetypeModel = JsonConvert.DeserializeObject<ArchetypeModel>(property.Value.ToString());
                        if (archetypeModel != null)
                        {
                            foreach (var fieldset in archetypeModel.Fieldsets)
                            {
                                if (fieldset.Properties.Where(p => p.Alias == fieldsetInfo[0].IdPropertyName && p.Value.ToString() == fieldsetInfo[0].IdPropertyValue).Any())
                                {
                                    setValueArchetypeFieldsetProperty(fieldset, fieldsetInfo[1].FieldsetName, value);
                                    content.SetValue(fieldsetInfo[0].FieldsetName, archetypeModel.SerializeForPersistence());
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        private static List<ArchetypePropertyInfo> ExtractPropertyInfo(string propertyAlias)
        {
            var result = new List<ArchetypePropertyInfo>();

            var properties = propertyAlias.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var property in properties)
            {
                ArchetypePropertyInfo archetypePropertyInfo = new ArchetypePropertyInfo();

                // TODO: Improve this code with regular expressions instead of using the string.Contains() method
                if (property.Contains("[") && property.Contains("=") && property.Contains("]"))
                {
                    try
                    {
                        archetypePropertyInfo.FieldsetName = property.Substring(0, property.IndexOf("["));
                        archetypePropertyInfo.IdPropertyName = property.Substring(property.IndexOf("[") + 1, property.IndexOf("=") - property.IndexOf("[") - 1);
                        archetypePropertyInfo.IdPropertyValue = property.Substring(property.IndexOf("=") + 1, property.IndexOf("]") - property.IndexOf("=") - 1);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(typeof(ArchetypeHelper), string.Format("Error extracting info from: {0}", propertyAlias), ex);
                        return new List<ArchetypePropertyInfo>();
                    }
                    if (!string.IsNullOrWhiteSpace(archetypePropertyInfo.FieldsetName) && !string.IsNullOrWhiteSpace(archetypePropertyInfo.IdPropertyName) && !string.IsNullOrWhiteSpace(archetypePropertyInfo.IdPropertyValue))
                    {
                        result.Add(archetypePropertyInfo);
                    }
                }
                else
                {
                    archetypePropertyInfo.FieldsetName = property;
                    result.Add(archetypePropertyInfo);
                }
            }
            return result;
        }

        private static void setValueArchetypeFieldsetProperty(ArchetypeFieldsetModel ArchetypeFieldset, string PropertyAlias, object value)
        {
            var property = ArchetypeFieldset.Properties.FirstOrDefault(x => x.Alias == PropertyAlias);
            property.Value = value;
        }

    }
}