'use strict';
(function () {

    //Main controller
    function uCKEditorDashboardController($log, $rootScope, $scope, dialogService, $window, assetsService) {

        // Load the css that converts the right hand side dialog into a full screen dialog
        assetsService.loadCss("/App_Plugins/uCKEditor/uCKEditor.dashboard.css");

        // This value is checked by the javascript executed when the user cicks on the button to check if the 
        // image picker is loaded
        $scope.frameLoaded = false;

        $scope.openMediaPicker = function () {

            // Open Umbraco's media picker dialog
            dialogService.mediaPicker({

                // Media picker dialog settings
                onlyImages: true,
                //showDetails: true,

                // Media picker callback
                callback: function (item) {

                    // Return the value to the iframe's parent posting a message
                    $window.parent.postMessage(item, '*');
                }
            });

            // Image picker is loaded
            $scope.frameLoaded = true;
        }

    };

    //register the controller
    angular.module("umbraco").controller('uCKEditor.DashboardController', uCKEditorDashboardController);

})();



