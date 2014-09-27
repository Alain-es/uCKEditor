'use strict';
(function () {

    // Create the controller
    function uCKEditorController($scope, assetsService, $log) {

        // Check whether the property editor's label should be hidden
        if ($scope.model.config.hideLabel == 1) {
            $scope.model.hideLabel = true;
        }

        // Tell the assetsService to load files required for the editor
        assetsService
            .load([
            '/App_Plugins/uCKEditor/CKEditor/ckeditor.js',
            '/App_Plugins/uCKEditor/CKEditor/config.js',
            '/App_Plugins/uCKEditor/CKEditor/lang/en.js',
            '/App_Plugins/uCKEditor/CKEditor/styles.js',
            '/App_Plugins/uCKEditor/CKEditor/plugins/image/dialogs/image.js'
            ])
            .then(function () {

                // This will be executed when all dependencies have loaded
                var editor;
                if ($scope.model.config.customConfigurationFile != null && jQuery.trim($scope.model.config.customConfigurationFile) != "") {
                    // Create the editor using the custom configuration file
                    editor = CKEDITOR.replace('ContentText_' + $scope.model.alias, {
                        customConfig: $scope.model.config.customConfigurationFile
                    });
                }
                else {
                    // Create the editor using the other setting
                    CKEDITOR.config.width = $scope.model.config.width;
                    CKEDITOR.config.height = $scope.model.config.height;
                    if ($scope.model.config.toolbarGroups != null && jQuery.trim($scope.model.config.toolbarGroups) != "") {
                        CKEDITOR.config.toolbarGroups = eval("[" + $scope.model.config.toolbarGroups + "]");
                    }
                    // Create the editor
                    editor = CKEDITOR.replace('ContentText_' + $scope.model.alias, CKEDITOR.config);
                }

                // Set editor's value when loading
                editor.setData($scope.model.value);

                // Save editor's value when submitting
                $scope.$on("formSubmitting", function (ev, args) {
                    $scope.model.value = editor.getData();
                });

            });

        // Load the separate css for the editor to avoid it blocking our js loading
        assetsService.loadCss('/App_Plugins/uCKEditor/CKEditor/skins/moono/editor.css');
        assetsService.loadCss('/App_Plugins/uCKEditor/CKEditor/skins/moono/editor.css');
    };

    // Register the controller
    angular.module("umbraco").controller('uCKEditor.uCKEditorController', uCKEditorController);

})();

