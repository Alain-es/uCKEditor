using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using umbraco;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.WebApi;
using umbraco.BusinessLogic;
using umbraco.BusinessLogic.Actions;

using uCKEditor.Helpers;

namespace uCKEditor
{
    public class InlineEditorHelper
    {

        public static string InlineEditorCreate(string htmlEditorContainerId, string contentPropertyAlias)
        {
            return InlineEditorCreate(htmlEditorContainerId, contentPropertyAlias, null);
        }

        public static string InlineEditorCreate(string htmlEditorContainerId, string contentPropertyAlias, string DatatypeAlias)
        {
            var result = string.Empty;
            try
            {
                // Check whether the current user has edit permissions in order to create the editor
                if (HasCurrentUserPermissionsToEditCurrentContentNode())
                {
                    // Check whether current page's content node is not null
                    UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                    if (umbracoHelper.AssignedContentItem != null)
                    {
                        // Get the content node
                        var content = ApplicationContext.Current.Services.ContentService.GetById(umbracoHelper.AssignedContentItem.Id);
                        if (content != null)
                        {
                            // Check whether the Property Datatype alias is provided when invoking this method
                            // If it is not the case then retrieve the Datatype alias from the content's property
                            if (DatatypeAlias == null || string.IsNullOrWhiteSpace(DatatypeAlias))
                            {
                                DatatypeAlias = content.Properties[contentPropertyAlias].Alias;
                            }

                            // Get the property data type settings
                            var propertyDataType = content.PropertyTypes.Where(p => p.Alias == DatatypeAlias).FirstOrDefault();
                            if (propertyDataType != null)
                            {
                                // Get the property prevalues in order to setup CKEditor
                                var datatypePrevalues = ApplicationContext.Current.Services.DataTypeService.GetPreValuesCollectionByDataTypeId(propertyDataType.DataTypeDefinitionId).PreValuesAsDictionary;

                                // Javascript statements to setup CKEditor
                                var script = new System.Text.StringBuilder();
                                script.AppendFormat(@"<script type='text/javascript'> 

                                                $(document).ready(function() {{

                                                    // Editor settings
                                                    editorSettings = {{
                                                        customConfigurationFile: ""{3}"",
                                                        width: ""{4}"",
                                                        height: ""{5}"",
                                                        language: ""{6}"",
                                                        font_names: ""{7}"", 
                                                        font_style: ""{8}"", 
                                                        format_tags: ""{9}"", 
                                                        allowedContent: ""{10}"", 
                                                        extraAllowedContent: ""{11}"", 
                                                        toolbar: ""{12}"",
                                                        toolbarGroups: ""{13}"",
                                                        removeButtons: ""{14}"", 
                                                        extraPlugins: ""{15}"", 
                                                        removePlugins: ""{16}""
                                                    }}; 

                                                    // Create inline editor
                                                    editor = createEditor('{0}', editorSettings);

                                                    // Add data required by the save button
                                                    $('#' + editor.name).attr('data-contentId', {1});
                                                    $('#' + editor.name).attr('data-contentPropertyAlias', '{2}');

                                                }} );

                                            </script>",
                                                htmlEditorContainerId,
                                                content.Id,
                                                contentPropertyAlias,
                                                processPreValue(datatypePrevalues, "customConfigurationFile"),
                                                processPreValue(datatypePrevalues, "width"),
                                                processPreValue(datatypePrevalues, "height"),
                                                processPreValue(datatypePrevalues, "language"),
                                                processPreValue(datatypePrevalues, "font_names"),
                                                processPreValue(datatypePrevalues, "font_style"),
                                                processPreValue(datatypePrevalues, "format_tags"),
                                                processPreValue(datatypePrevalues, "allowedContent"),
                                                processPreValue(datatypePrevalues, "extraAllowedContent"),
                                                processPreValue(datatypePrevalues, "toolbar"),
                                                processPreValue(datatypePrevalues, "toolbarGroups"),
                                                processPreValue(datatypePrevalues, "removeButtons"),
                                                processPreValue(datatypePrevalues, "extraPlugins"),
                                                processPreValue(datatypePrevalues, "removePlugins")
                                );
                                result = script.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static string InlineEditorInitialize(bool loadJquery = true, bool loadJqueryUI = true)
        {
            var result = new System.Text.StringBuilder();

            // Check whether the current user has permissions to edit the current node
            if (HasCurrentUserPermissionsToEditCurrentContentNode())
            {
                // Check wheter JQuery should be loaded or not
                if (loadJquery)
                {
                    result.Append(@" <script type='text/javascript' src='/App_Plugins/uCKEditor/jquery/jquery-1.11.1.min.js'></script> ");
                }

                // Check wheter JQueryUI should be loaded or not
                if (loadJqueryUI)
                {
                    result.Append(@" <script type='text/javascript' src='/App_Plugins/uCKEditor/jquery/jquery-ui-1.11.2.min.js'></script> ");
                    result.Append(@" <link rel='stylesheet' href='/App_Plugins/uCKEditor/jquery/jquery-ui-1.11.2-theme-smoothness.css'> ");
                }

                // Editor's scripts
                result.Append(@" <script type='text/javascript' src='/App_Plugins/uCKEditor/jquery/jquery.cookie.js'></script> ");
                result.Append(@" <script type='text/javascript' src='/App_Plugins/uCKEditor/CKEditor/ckeditor.js'></script> ");
                result.Append(@" <script type='text/javascript' src='/App_Plugins/uCKEditor/uCKEditor.js'></script> ");
                
                result.Append(@"<script type='text/javascript'> 

                                    // Load dinamically CSS stylesheet
                                    var fileref=document.createElement('link');
                                    fileref.setAttribute('rel', 'stylesheet');
                                    fileref.setAttribute('type', 'text/css');
                                    fileref.setAttribute('href', '/App_Plugins/uCKEditor/uCKEditor.editor.css');
                                    document.getElementsByTagName('head')[0].appendChild(fileref);

                                    $(document).ready(function() {
                                        CKEDITOR.disableAutoInline = false;

                                        // Set the default jquery ajax headers to include csrf token
                                        // Need to use the beforeSend method because the token might change (different for each logged in user)
                                        $.ajaxSetup({
                                            beforeSend: function (xhr) {
                                                xhr.setRequestHeader('X-XSRF-TOKEN', $.cookie('XSRF-TOKEN'));
                                            }
                                        });

                                    } );
                                </script>");
            }
            return result.ToString();
        }

        private static bool HasCurrentUserPermissionsToEditCurrentContentNode()
        {
            var result = false; ;

            // Get current user
            User user = umbraco.helper.GetCurrentUmbracoUser();

            // Get current content node
            UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            int nodeId = Constants.System.Root;
            if (umbracoHelper.AssignedContentItem != null)
            {
                nodeId = umbracoHelper.AssignedContentItem.Id;
            }

            // Check whether there is an user logged in
            if (user != null)
            {
                // Check whether the user has permissions to edit the content node
                result = PermissionHelper.CheckContentPermissions(user.Id, nodeId, new char[] { ActionUpdate.Instance.Letter });
            }
            return result;
        }

        private static string processPreValue(IDictionary<string, Umbraco.Core.Models.PreValue> datatypePrevalues, string propertyAlias)
        {
            var result = string.Empty;
            if (datatypePrevalues.ContainsKey(propertyAlias) && datatypePrevalues[propertyAlias].Value != null)
            {
                result = datatypePrevalues[propertyAlias].Value.Replace("\n", string.Empty);
            }
            return result;
        }

    }
}