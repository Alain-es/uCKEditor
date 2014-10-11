angular.module("umbraco")
.controller("uCKEditor.uCKEditorController",
function ($scope, assetsService, dialogService, $log) {

    // Check whether the property editor's label should be hidden
    if ($scope.model.config.hideLabel == 1) {
        $scope.model.hideLabel = true;
    }

    // Tell the assetsService to load files required for the editor
    assetsService
    .loadJs([
        '/App_Plugins/uCKEditor/CKEditor/ckeditor.js',
    ])
        .then(function () {

            // Textaread ID used by the editor
            var editorTextAreaId = 'editorPlaceholder';

            // Assign a UniqueId for the textarea (in case there is more than one editor in the same form)
            var date = new Date();
            var uniqueID = "" + date.getDate() + date.getHours() + date.getMinutes() + date.getSeconds() + date.getMilliseconds();
            $('#' + editorTextAreaId).attr("id", uniqueID);
            editorTextAreaId = uniqueID;

            // Create the CKEditor control
            var editor;
            // Loads plugins to insert images from the Umbraco Media Library and to save changes 
            CKEDITOR.config.extraPlugins = 'umbracomedia';
            if ($scope.model.config.customConfigurationFile != null && jQuery.trim($scope.model.config.customConfigurationFile) != '') {
                // Create the editor using the custom configuration file
                editor = CKEDITOR.replace(editorTextAreaId, {
                    customConfig: $scope.model.config.customConfigurationFile
                });
            }
            else {
                // Create the editor using the other setting
                CKEDITOR.config.width = $scope.model.config.width;
                CKEDITOR.config.height = $scope.model.config.height;
                CKEDITOR.config.language = $scope.model.config.language;
                if ($scope.model.config.toolbarGroups != null && jQuery.trim($scope.model.config.toolbarGroups) != '') {
                    CKEDITOR.config.toolbarGroups = eval('[' + $scope.model.config.toolbarGroups + ',]');
                }
                if ($scope.model.config.toolbar != null && jQuery.trim($scope.model.config.toolbar) != '') {
                    CKEDITOR.config.toolbar = eval('[' + $scope.model.config.toolbar + ',]');
                }
                editor = CKEDITOR.replace(editorTextAreaId, CKEDITOR.config);
            }

            // Get the internal texteditor ID 
            var editorId = 'cke_' + editorTextAreaId;

            // Get UmbracoMedia plugin's button IDs
            var editorButtonMediaIdSelector = '#' + editorId + ' .cke_button__umbracomedia';

            // Hook the click event for the UmbracoMedia plugin's button
            $(document).on('click', editorButtonMediaIdSelector, function () {

                // Open Umbraco's media picker dialog
                dialogService.mediaPicker({
                    // Media picker dialog settings
                    onlyImages: true,
                    showDetails: true,

                    // Media picker callback
                    callback: function (data) {

                        // Check whether an image has been selected
                        if (data) {

                            // Selected image
                            var selectedImage = {
                                alt: data.altText,
                                src: (data.url) ? data.url : '/App_Plugins/uCKEditor/CKEditor/plugins/umbracomedia/images/noimage.png',
                                rel: data.id
                            };

                            // Create an html img tag with the picked image properties to insert into the editor
                            var htmlImage = editor.document.createElement('img');
                            htmlImage.setAttribute('src', selectedImage.src);
                            htmlImage.setAttribute('alt', selectedImage.alt);
                            editor.insertElement(htmlImage)
                        };
                    }
                });

            });

            // Set editor's value (when loading)
            editor.setData($scope.model.value);

            // Save editor's value (when submitting)
            $scope.$on("formSubmitting", function (ev, args) {
                $scope.model.value = editor.getData();
            });

            // Hook the destroy event in order to destroy the CKEditor instances
            $scope.$on("$destroy", function () {
                // Destroy all CKEditors
                for (var instanceName in CKEDITOR.instances) {
                    var instanceEditor = CKEDITOR.instances[instanceName];
                    if (instanceEditor) {
                        instanceEditor.destroy(true);
                        instanceEditor = null;
                    }
                }
            });
        });
});
