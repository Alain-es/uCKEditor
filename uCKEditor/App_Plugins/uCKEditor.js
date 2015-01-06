
/* Settings structure:

        editorSettings = {
            customConfigurationFile: "",
            width: "",
            height: "",
            language: "",
            font_names: "",
            font_style: "",
            format_tags: "",
            allowedContent: "",
            extraAllowedContent: "",
            toolbar: "",
            toolbarGroups: "",
            removeButtons: "",
            extraPlugins: "",
            removePlugins: ""
        } 

*/

function createEditor(editorPlaceholderId, editorSettings) {

    // Textaread ID used by the editor
    var editorTextAreaId = editorPlaceholderId;

    // Assign a UniqueId for the textarea (in case there is more than one editor in the same form)
    var date = new Date();
    var uniqueID = "" + date.getDate() + date.getHours() + date.getMinutes() + date.getSeconds() + date.getMilliseconds();
    $('#' + editorTextAreaId).attr('id', uniqueID);
    editorTextAreaId = uniqueID;

    // Add the attribute: contenteditable="true" 
    $('#' + editorTextAreaId).attr('contenteditable', 'true');

    // Create the inline CKEditor control
    var editor;

    // Loads plugins to insert images from the Umbraco Media Library and to save changes 
    if (CKEDITOR.config.plugins != null && CKEDITOR.config.plugins != 'undefined' && jQuery.trim(CKEDITOR.config.plugins) != '')
        CKEDITOR.config.plugins += ',umbracomedia,umbracosave';
    else
        CKEDITOR.config.plugins = 'umbracomedia,umbracosave';

    if (editorSettings.customConfigurationFile != null && jQuery.trim(editorSettings.customConfigurationFile) != '') {
        // Create the editor using the custom configuration file
        editor = CKEDITOR.inline(editorTextAreaId, {
            customConfig: editorSettings.customConfigurationFile
        });
    }
    else {
        // Create the editor using the other setting
        CKEDITOR.config.width = editorSettings.width;
        CKEDITOR.config.height = editorSettings.height;
        if (editorSettings.language != null && jQuery.trim(editorSettings.language) != '') {
            CKEDITOR.config.language = editorSettings.language;
        }
        if (editorSettings.font_names != null && jQuery.trim(editorSettings.font_names) != '') {
            CKEDITOR.config.font_names = editorSettings.font_names;
        }
        if (editorSettings.font_style != null && jQuery.trim(editorSettings.font_style) != '') {
            CKEDITOR.config.font_style = editorSettings.font_style;
        }
        if (editorSettings.format_tags != null && jQuery.trim(editorSettings.format_tags) != '') {
            CKEDITOR.config.format_tags = editorSettings.format_tags;
        }
        if (editorSettings.allowedContent != null && jQuery.trim(editorSettings.allowedContent) != '') {
            CKEDITOR.config.allowedContent = editorSettings.allowedContent;
        }
        if (editorSettings.extraAllowedContent != null && jQuery.trim(editorSettings.extraAllowedContent) != '') {
            CKEDITOR.config.extraAllowedContent = editorSettings.extraAllowedContent;
        }
        if (editorSettings.toolbar != null && jQuery.trim(editorSettings.toolbar) != '') {
            CKEDITOR.config.toolbar = eval("[['umbracosave','umbracomedia'], " + editorSettings.toolbar + ",]");
        }
        if (editorSettings.toolbarGroups != null && jQuery.trim(editorSettings.toolbarGroups) != '') {
            CKEDITOR.config.toolbarGroups = eval("[{name: 'umbraco', groups: ['umbraco']}, " + editorSettings.toolbarGroups + ",]");
        }
        if (editorSettings.removeButtons != null && jQuery.trim(editorSettings.removeButtons) != '') {
            CKEDITOR.config.removeButtons = editorSettings.removeButtons;
        }
        if (editorSettings.extraPlugins != null && jQuery.trim(editorSettings.extraPlugins) != '') {
            CKEDITOR.config.extraPlugins = editorSettings.extraPlugins;
        }
        if (editorSettings.removePlugins != null && jQuery.trim(editorSettings.removePlugins) != '') {
            CKEDITOR.config.removePlugins = editorSettings.removePlugins;
        }
        editor = CKEDITOR.inline(editorTextAreaId, CKEDITOR.config);
    }

    // If toolbars haven't been customized then add umbraco toolbar group to the default CKEditor toolbar
    if ((CKEDITOR.config.toolbarGroups == null || CKEDITOR.config.toolbarGroups == 'undefined' || jQuery.trim(CKEDITOR.config.toolbarGroups) == '') &&
        (CKEDITOR.config.toolbar == null || CKEDITOR.config.toolbar == 'undefined' || jQuery.trim(CKEDITOR.config.toolbar) == '')) {
        editor.ui.addToolbarGroup('umbraco', 0);
    }

    // Get the internal texteditor ID 
    var editorId = 'cke_' + editorTextAreaId;

    // Get UmbracoMedia plugin's button IDs
    var editorButtonMediaIdSelector = '#' + editorId + ' .cke_button__umbracomedia';

    // Hook the click event for the UmbracoMedia plugin's button
    $(document).on('click', editorButtonMediaIdSelector, function () {

        // The iframe is initially hidden in order to avoid the user to see how umbraco backoffice is loading
        var iframe = $('<iframe id="frameBackoffice" frameborder="0" marginwidth="0" marginheight="0" src="/umbraco/#/uckeditor" style="display:none;"></iframe>');
        var dialog = $("<div style=''></div>").append(iframe).appendTo("body").dialog({
            autoOpen: false,
            modal: true,
            resizable: true,
            width: 800,
            height: 600,
            close: function () {
                // Destroy the iframe
                iframe.remove();
            }
        });

        // Load the iframe within a modal dialog
        var iframe2 = dialog.dialog("option", "title", 'Umbraco media').dialog("open");
        iframe2.ready(function () {

            var iframecontents = $("#frameBackoffice").contents();
            var iframe = document.getElementById("frameBackoffice");

            var doc = (iframe.contentWindow || iframe.contentDocument);
            if (doc.document) doc = doc.document;

            var interval = null;

            // Setup a 5seconds timeout to load the umbraco backoffice in the iframe and display the umbraco dialog
            var timer = setTimeout(function () {
                clearInterval(interval);

                // Close the dialog (and the ifram)
                iframe2.dialog('close');
            }, 5000);

            $("#frameBackoffice").contents().find("#mainwrapper").hide();
            $("#frameBackoffice").contents().find("#btnMediaPicker").click();

            interval = setInterval(function () {
                $("#frameBackoffice").contents().find("#mainwrapper").hide();
                $("#frameBackoffice").contents().find("#btnMediaPicker").click();

                // Get dashboard controller angular scope
                var scope = null;
                if (iframe != null && iframe.contentWindow != null && iframe.contentWindow.angular != null) {
                    scope = iframe.contentWindow.angular.element("#formDashboard").scope();
                    if (scope != null) {
                        scope.openMediaPicker();
                    }
                }
                // Check whether the media dialog is loaded
                if (scope != null && scope.frameLoaded != false) {
                    // Stop the timeout and the interval
                    clearTimeout(timer);
                    clearInterval(interval);
                    // Now that everything is loaded, display the iframe
                    $("#frameBackoffice").show();
                }
            }, 50);


            // Listen to message from the iframe
            eventListener = window.addEventListener("message", receiveMessage, false);

            function receiveMessage(event) {
                //if (event.origin !== "http://domain.com:80")
                //    return;

                // Create an html img tag with the picked image properties to insert into the editor
                var htmlImage = editor.document.createElement('img');
                htmlImage.setAttribute('src', event.data.image);
                //htmlImage.setAttribute('alt', event.data.alt);
                editor.insertElement(htmlImage)

                // Remove the event listener
                window.removeEventListener("message", receiveMessage);

                // Close the dialog (and the ifram)
                iframe2.dialog('close');
            }
        });

    });

    // Get UmbracoSave plugin's button IDs
    var editorButtonSaveIdSelector = '#' + editorId + ' .cke_button__umbracosave';

    // Hook the click event for the UmbracoSave plugin's button
    $(document).on('click', editorButtonSaveIdSelector, function () {
        var contentId = $('#' + editorTextAreaId).attr('data-contentId');
        var contentPropertyAlias = $('#' + editorTextAreaId).attr('data-contentPropertyAlias');
        // Save editor's value 
        getSavedContentPropertyValue(contentId, contentPropertyAlias, editor.getData());
    });

    return editor;
}


//function getContentPropertyValue(editor, contentId, propertyAlias) {
//    $.ajax({
//        url: "/umbraco/backoffice/uCKEditor/uCKEditorApi/getContentPropertyValue",
//        data: {
//            contentId: contentId,
//            propertyAlias: propertyAlias
//        },
//        type: "GET",
//        dataType: "html",
//        contentType: 'application/json; charset=utf-8',
//        success: function (result, status, xhr) {
//            // Strip ")]}'," from the response (always at the beginning)
//            if (result.indexOf(")]}',\n") == 0) {
//                result = result.substring(6);
//            }
//            // Remove the double quotes at the beginning and at the end
//            result = result.substring(1);
//            result = result.substring(0, result.length - 1);
//            // Set editor value
//            editor.setData(result);
//            //console.log(result);
//        },
//        error: function (xhr, status, error) {
//            console.log(xhr.responseText);
//        }
//    });

//};


function getSavedContentPropertyValue(contentId, propertyAlias, propertyValue) {

    $.ajax({
        url: "/umbraco/backoffice/uCKEditor/uCKEditorApi/getSavedContentPropertyValue",
        data: {
            contentId: contentId,
            propertyAlias: propertyAlias,
            propertyValue: propertyValue
        },
        type: "GET",
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (result, status, xhr) {
            //console.log(status);
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseText);
        }
    });

};



