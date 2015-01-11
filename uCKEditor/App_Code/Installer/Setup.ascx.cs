using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Umbraco.Core;
using Umbraco.Core.Models;

namespace uCKEditor.Installer
{
    public partial class Setup : System.Web.UI.UserControl
    {
        private const string PropertyEditorAlias = "uCKEditor";

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnFinish_Click(object sender, EventArgs e)
        {
            // Create new uCKEditor data type
            if (chkCreateNewDataType.Checked)
            {
                // Check wheter a uCKEditor data type already exists before creating the default one
                var dataTypeDefinitions = ApplicationContext.Current.Services.DataTypeService.GetDataTypeDefinitionByPropertyEditorAlias(PropertyEditorAlias);
                if (dataTypeDefinitions == null || !dataTypeDefinitions.Any())
                {
                    // Datatype
                    var dataTypeDefinition = new DataTypeDefinition(-1, PropertyEditorAlias)
                    {
                        DatabaseType = DataTypeDatabaseType.Ntext,
                        Name = PropertyEditorAlias
                    };
                    // Prevalues
                    var dataTypePreValues = new Dictionary<string, PreValue>();
                    // PreValue Id="1" Alias="hideLabel"
                    dataTypePreValues.Add("hideLabel", new PreValue(1, "1"));
                    // PreValue Id="2" Alias="customConfigurationFile" 
                    dataTypePreValues.Add("customConfigurationFile", new PreValue(2, ""));
                    // PreValue Id="3" Alias="width" 
                    dataTypePreValues.Add("width", new PreValue(3, ""));
                    // PreValue Id="4" Alias="height" 
                    dataTypePreValues.Add("height", new PreValue(4, "500"));
                    // PreValue Id="5" Alias="language" 
                    dataTypePreValues.Add("language", new PreValue(5, "en"));
                    // PreValue Id="6" Alias="font_names"
                    dataTypePreValues.Add("font_names", new PreValue(6, ""));
                    // PreValue Id="7" Alias="font_style" 
                    dataTypePreValues.Add("font_style", new PreValue(7, ""));
                    // PreValue Id="8" Alias="format_tags" 
                    dataTypePreValues.Add("format_tags", new PreValue(8, ""));
                    // PreValue Id="9" Alias="allowedContent" 
                    dataTypePreValues.Add("allowedContent", new PreValue(9, ""));
                    // PreValue Id="10" Alias="extraAllowedContent" 
                    dataTypePreValues.Add("extraAllowedContent", new PreValue(10, ""));
                    // PreValue Id="11" Alias="toolbar" 
                    dataTypePreValues.Add("toolbar", new PreValue(11, ""));
                    // PreValue Id="12" Alias="toolbarGroups" 
                    dataTypePreValues.Add("toolbarGroups", new PreValue(12, ""));
                    // PreValue Id="13" Alias="removeButtons" 
                    dataTypePreValues.Add("removeButtons", new PreValue(13, "NewPage,Preview,Templates,Save,Form,Checkbox,Radio,TextField,Textarea,Select,Button,ImageButton,HiddenField,Flash,IFrame"));
                    // PreValue Id="14" Alias="extraPlugins" 
                    dataTypePreValues.Add("extraPlugins", new PreValue(14, ""));
                    // PreValue Id="15" Alias="removePlugins" 
                    dataTypePreValues.Add("removePlugins", new PreValue(15, ""));

                    // Save changes
                    ApplicationContext.Current.Services.DataTypeService.SaveDataTypeAndPreValues(dataTypeDefinition, dataTypePreValues, 0);
                }
            }

            // Hide the panel
            pnlFinishing.Visible = false;
            pnlFinished.Visible = true;

        }

    }
}